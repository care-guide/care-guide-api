using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Models.Entities;
using CareGuide.Security.Interfaces;

namespace CareGuide.Tests.Services;

public class RefreshTokenServiceTests
{
    private readonly IRefreshTokenRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly RefreshTokenService _sut;

    public RefreshTokenServiceTests()
    {
        _repository = Substitute.For<IRefreshTokenRepository>();
        _jwtService = Substitute.For<IJwtService>();
        _sut = new RefreshTokenService(_repository, _jwtService);
    }

    [Fact(DisplayName = "CreateAsync: generates token with correct properties")]
    public async Task CreateAsync_GeneratesTokenWithCorrectProperties()
    {
        var userId = Guid.NewGuid();
        _jwtService.GenerateRefreshToken().Returns("generated-token");

        var result = await _sut.CreateAsync(userId, CancellationToken.None);

        result.UserId.Should().Be(userId);
        result.Token.Should().Be("generated-token");
        result.Revoked.Should().BeFalse();
        result.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        await _repository.Received(1).AddAsync(result, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "GetAsync: delegates to repository")]
    public async Task GetAsync_DelegatesToRepository()
    {
        var userId = Guid.NewGuid();
        var token = "some-token";
        var stored = new RefreshToken { UserId = userId, Token = token };
        _repository.GetByTokenAsync(userId, token, Arg.Any<CancellationToken>()).Returns(stored);

        var result = await _sut.GetAsync(userId, token, CancellationToken.None);

        result.Should().Be(stored);
    }

    [Fact(DisplayName = "RotateAsync: token not found throws UnauthorizedAccessException")]
    public async Task RotateAsync_TokenNotFound_ThrowsUnauthorized()
    {
        var userId = Guid.NewGuid();
        _repository.GetByTokenAsync(userId, "bad-token", Arg.Any<CancellationToken>()).Returns((RefreshToken?)null);

        var act = async () => await _sut.RotateAsync(userId, "bad-token", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("*Invalid or expired*");
    }

    [Fact(DisplayName = "RotateAsync: expired token throws UnauthorizedAccessException")]
    public async Task RotateAsync_ExpiredToken_ThrowsUnauthorized()
    {
        var userId = Guid.NewGuid();
        var expired = new RefreshToken { UserId = userId, Token = "tok", ExpiresAt = DateTime.UtcNow.AddDays(-1), Revoked = false };
        _repository.GetByTokenAsync(userId, "tok", Arg.Any<CancellationToken>()).Returns(expired);

        var act = async () => await _sut.RotateAsync(userId, "tok", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "RotateAsync: revoked token throws UnauthorizedAccessException")]
    public async Task RotateAsync_RevokedToken_ThrowsUnauthorized()
    {
        var userId = Guid.NewGuid();
        var revoked = new RefreshToken { UserId = userId, Token = "tok", ExpiresAt = DateTime.UtcNow.AddDays(1), Revoked = true };
        _repository.GetByTokenAsync(userId, "tok", Arg.Any<CancellationToken>()).Returns(revoked);

        var act = async () => await _sut.RotateAsync(userId, "tok", CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>();
    }

    [Fact(DisplayName = "RotateAsync: valid token revokes old and creates new token")]
    public async Task RotateAsync_ValidToken_RevokesOldAndCreatesNew()
    {
        var userId = Guid.NewGuid();
        var old = new RefreshToken { UserId = userId, Token = "old-tok", ExpiresAt = DateTime.UtcNow.AddDays(1), Revoked = false };
        _repository.GetByTokenAsync(userId, "old-tok", Arg.Any<CancellationToken>()).Returns(old);
        _jwtService.GenerateRefreshToken().Returns("new-tok");

        var result = await _sut.RotateAsync(userId, "old-tok", CancellationToken.None);

        old.Revoked.Should().BeTrue();
        result.Token.Should().Be("new-tok");
        result.Revoked.Should().BeFalse();
        result.UserId.Should().Be(userId);
        await _repository.Received(1).InvalidateAndReplaceAsync(old, result, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "InvalidateAsync: token found sets revoked and calls InvalidateAndReplace")]
    public async Task InvalidateAsync_TokenFound_RevokesIt()
    {
        var userId = Guid.NewGuid();
        var stored = new RefreshToken { UserId = userId, Token = "tok", Revoked = false };
        _repository.GetByTokenAsync(userId, "tok", Arg.Any<CancellationToken>()).Returns(stored);

        await _sut.InvalidateAsync(userId, "tok", CancellationToken.None);

        stored.Revoked.Should().BeTrue();
        await _repository.Received(1).InvalidateAndReplaceAsync(stored, stored, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "InvalidateAsync: token not found does nothing")]
    public async Task InvalidateAsync_TokenNotFound_DoesNothing()
    {
        var userId = Guid.NewGuid();
        _repository.GetByTokenAsync(userId, "missing", Arg.Any<CancellationToken>()).Returns((RefreshToken?)null);

        await _sut.InvalidateAsync(userId, "missing", CancellationToken.None);

        await _repository.DidNotReceive().InvalidateAndReplaceAsync(Arg.Any<RefreshToken>(), Arg.Any<RefreshToken>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "InvalidateAllAsync: marks all user tokens as revoked and saves")]
    public async Task InvalidateAllAsync_RevokesAllUserTokens()
    {
        var userId = Guid.NewGuid();
        var tokens = new List<RefreshToken>
        {
            new() { UserId = userId, Token = "tok1", Revoked = false },
            new() { UserId = userId, Token = "tok2", Revoked = false },
        };
        _repository.GetAllByUserAsync(userId, Arg.Any<CancellationToken>()).Returns(tokens);

        await _sut.InvalidateAllAsync(userId, CancellationToken.None);

        tokens.Should().OnlyContain(t => t.Revoked == true);
        await _repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
