using AutoMapper;
using CareGuide.Core.Interfaces;
using CareGuide.Core.Services;
using CareGuide.Infra.Interfaces;
using CareGuide.Infra.TransactionManagement;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.DTOs.Person;
using CareGuide.Models.DTOs.User;
using CareGuide.Models.Entities;
using CareGuide.Models.Enums;
using CareGuide.Security.Interfaces;

namespace CareGuide.Tests.Services;

public class AccountServiceTests
{
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IPersonService _personService;
    private readonly IEfTransactionUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IMapper _mapper;
    private readonly AccountService _sut;

    private static readonly PersonDto DefaultPersonDto = new(
        Guid.NewGuid(), "Test User", null, Gender.F, DateOnly.FromDateTime(DateTime.Now.AddYears(-20)), DateTime.UtcNow, DateTime.UtcNow
    );

    private static readonly UserDto DefaultUserDto = new(
        Guid.NewGuid(), DefaultPersonDto.Id, "test@test.com", DateTime.UtcNow, DateTime.UtcNow
    );

    public AccountServiceTests()
    {
        _userService = Substitute.For<IUserService>();
        _userRepository = Substitute.For<IUserRepository>();
        _personService = Substitute.For<IPersonService>();
        _unitOfWork = Substitute.For<IEfTransactionUnitOfWork>();
        _jwtService = Substitute.For<IJwtService>();
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _mapper = Substitute.For<IMapper>();
        _sut = new AccountService(_userService, _userRepository, _personService, _unitOfWork, _jwtService, _refreshTokenService, _mapper);
    }

    [Fact(DisplayName = "CreateAccountAsync: valid data creates person, user, tokens, and returns AccountDto")]
    public async Task CreateAccountAsync_ValidData_ReturnsAccountDto()
    {
        var createDto = new CreateAccountDto("new@test.com", "Str0ng@Pass!", "Test User", Gender.F, DateOnly.FromDateTime(DateTime.Now));
        var refreshToken = new RefreshToken { UserId = DefaultUserDto.Id, Token = "refresh-token" };

        _mapper.Map<CreatePersonDto>(createDto).Returns(new CreatePersonDto(Guid.NewGuid(), "Test User", Gender.F, DateOnly.FromDateTime(DateTime.Now), null));
        _mapper.Map<CreateUserDto>(createDto).Returns(new CreateUserDto(Guid.NewGuid(), DefaultPersonDto.Id, "new@test.com", "Str0ng@Pass!"));
        _personService.CreateAsync(Arg.Any<CreatePersonDto>(), Arg.Any<CancellationToken>()).Returns(DefaultPersonDto);
        _userService.CreateAsync(DefaultPersonDto, Arg.Any<CreateUserDto>(), Arg.Any<CancellationToken>()).Returns(DefaultUserDto);
        _jwtService.GenerateToken(DefaultUserDto.Id, DefaultPersonDto.Id, DefaultUserDto.Email).Returns("access-token");
        _refreshTokenService.CreateAsync(DefaultUserDto.Id, Arg.Any<CancellationToken>()).Returns(refreshToken);

        var result = await _sut.CreateAccountAsync(createDto, CancellationToken.None);

        result.Email.Should().Be(DefaultUserDto.Email);
        result.SessionToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "CreateAccountAsync: exception triggers transaction rollback")]
    public async Task CreateAccountAsync_ServiceThrows_RollsBackTransaction()
    {
        var createDto = new CreateAccountDto("fail@test.com", "Str0ng@Pass!", "User", Gender.M, DateOnly.FromDateTime(DateTime.Now));

        _mapper.Map<CreatePersonDto>(createDto).Returns(new CreatePersonDto(Guid.NewGuid(), "User", Gender.M, DateOnly.FromDateTime(DateTime.Now), null));
        _mapper.Map<CreateUserDto>(createDto).Returns(new CreateUserDto(Guid.NewGuid(), Guid.NewGuid(), "fail@test.com", "Str0ng@Pass!"));
        _personService.CreateAsync(Arg.Any<CreatePersonDto>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<PersonDto>(new InvalidOperationException("Email already registered")));

        var act = async () => await _sut.CreateAccountAsync(createDto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
        await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "LoginAccountAsync: user not found throws InvalidOperationException")]
    public async Task LoginAccountAsync_UserNotFound_ThrowsInvalidOperationException()
    {
        var loginDto = new LoginAccountDto("notfound@test.com", "Str0ng@Pass!");
        _userRepository.GetByEmailAsync(loginDto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.LoginAccountAsync(loginDto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Wrong password or email");
    }

    [Fact(DisplayName = "LoginAccountAsync: wrong password throws InvalidOperationException")]
    public async Task LoginAccountAsync_WrongPassword_ThrowsInvalidOperationException()
    {
        var loginDto = new LoginAccountDto("user@test.com", "WrongP@ss1");
        var hashedCorrectPassword = Security.Helpers.PasswordManagerHelper.HashPassword("Correct@Pass1");
        var user = new User
        {
            Id = Guid.NewGuid(),
            PersonId = DefaultPersonDto.Id,
            Email = "user@test.com",
            Password = hashedCorrectPassword
        };

        _userRepository.GetByEmailAsync(loginDto.Email, Arg.Any<CancellationToken>()).Returns(user);

        var act = async () => await _sut.LoginAccountAsync(loginDto, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Wrong password or email");
    }

    [Fact(DisplayName = "LoginAccountAsync: valid credentials returns AccountDto with tokens")]
    public async Task LoginAccountAsync_ValidCredentials_ReturnsAccountDto()
    {
        var plainPassword = "Str0ng@Pass!";
        var hashedPassword = Security.Helpers.PasswordManagerHelper.HashPassword(plainPassword);
        var loginDto = new LoginAccountDto("user@test.com", plainPassword);
        var user = new User
        {
            Id = DefaultUserDto.Id,
            PersonId = DefaultPersonDto.Id,
            Email = "user@test.com",
            Password = hashedPassword
        };
        var refreshToken = new RefreshToken { UserId = user.Id, Token = "refresh-token" };

        _userRepository.GetByEmailAsync(loginDto.Email, Arg.Any<CancellationToken>()).Returns(user);
        _jwtService.GenerateToken(user.Id, user.PersonId, loginDto.Email).Returns("access-token");
        _refreshTokenService.CreateAsync(user.Id, Arg.Any<CancellationToken>()).Returns(refreshToken);
        _userService.GetByIdDtoAsync(user.Id, Arg.Any<CancellationToken>()).Returns(DefaultUserDto);
        _personService.GetAsync(user.PersonId!.Value, Arg.Any<CancellationToken>()).Returns(DefaultPersonDto);

        var result = await _sut.LoginAccountAsync(loginDto, CancellationToken.None);

        result.SessionToken.Should().Be("access-token");
        result.RefreshToken.Should().Be("refresh-token");
    }

    [Fact(DisplayName = "RefreshTokenAsync: user not found throws UnauthorizedAccessException")]
    public async Task RefreshTokenAsync_UserNotFound_ThrowsUnauthorizedAccessException()
    {
        var dto = new RefreshTokenDto("old-token", "notfound@test.com");
        _userRepository.GetByEmailAsync(dto.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        var act = async () => await _sut.RefreshTokenAsync(dto, CancellationToken.None);

        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("*email*");
    }

    [Fact(DisplayName = "LogoutAccountAsync: calls invalidate all tokens for user")]
    public async Task LogoutAccountAsync_CallsInvalidateAll()
    {
        var userId = Guid.NewGuid();

        await _sut.LogoutAccountAsync(userId, CancellationToken.None);

        await _refreshTokenService.Received(1).InvalidateAllAsync(userId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAccountAsync: user not found throws KeyNotFoundException")]
    public async Task DeleteAccountAsync_UserNotFound_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        _userService.GetByIdAsync(id, Arg.Any<CancellationToken>())
            .Returns(Task.FromException<User>(new KeyNotFoundException()));

        var act = async () => await _sut.DeleteAccountAsync(id, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
        await _unitOfWork.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "DeleteAccountAsync: deletes user and associated person")]
    public async Task DeleteAccountAsync_ValidUser_DeletesUserAndPerson()
    {
        var userId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var user = new User { Id = userId, PersonId = personId, Email = "test@test.com", Password = "hash" };

        _userService.GetByIdAsync(userId, Arg.Any<CancellationToken>()).Returns(user);

        await _sut.DeleteAccountAsync(userId, CancellationToken.None);

        await _userService.Received(1).DeleteAsync(userId, Arg.Any<CancellationToken>());
        await _personService.Received(1).DeleteAsync(personId, Arg.Any<CancellationToken>());
        await _unitOfWork.Received(1).CommitTransactionAsync(Arg.Any<CancellationToken>());
    }
}
