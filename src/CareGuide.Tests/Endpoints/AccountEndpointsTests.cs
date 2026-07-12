using CareGuide.API.Endpoints;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Account;
using CareGuide.Models.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CareGuide.Tests.Endpoints;

public class AccountEndpointsTests
{
    private readonly IAccountService _accountService;

    private static readonly AccountDto DefaultAccountDto = new(
        Guid.NewGuid(),
        "user@test.com",
        "access-token",
        "refresh-token",
        "Test User",
        Gender.F,
        DateOnly.FromDateTime(DateTime.Now.AddYears(-20))
    );

    public AccountEndpointsTests()
    {
        _accountService = Substitute.For<IAccountService>();
    }

    private static DefaultHttpContext NewHttpContext(bool nativeClient = false)
    {
        var context = new DefaultHttpContext();
        if (nativeClient)
            context.Request.Headers[AuthClientConstants.ClientTypeHeaderName] = AuthClientConstants.MobileClientTypeValue;

        return context;
    }

    [Fact(DisplayName = "Create: browser client (no header) gets AccountResponseDto with cookies, no tokens in body")]
    public async Task Create_BrowserClient_ReturnsProfileOnlyBodyAndSetsCookies()
    {
        var createDto = new CreateAccountDto("user@test.com", "Str0ng@Pass!", "Test User", Gender.F, DateOnly.FromDateTime(DateTime.Now));
        _accountService.CreateAccountAsync(createDto, Arg.Any<CancellationToken>()).Returns(DefaultAccountDto);
        var httpContext = NewHttpContext(nativeClient: false);

        var result = await AccountEndpoints.Create(createDto, _accountService, httpContext, CancellationToken.None);

        var created = result.Should().BeOfType<Created<AccountResponseDto>>().Subject;
        created.Value.Should().NotBeNull();
        created.Value!.Email.Should().Be(DefaultAccountDto.Email);

        var setCookieHeader = httpContext.Response.Headers.SetCookie.ToString();
        setCookieHeader.Should().Contain("sessionToken=access-token");
        setCookieHeader.Should().Contain("refreshToken=refresh-token");
    }

    [Fact(DisplayName = "Create: native client (X-Client-Type: mobile) gets tokens in body plus cookies")]
    public async Task Create_NativeClient_ReturnsTokensInBody()
    {
        var createDto = new CreateAccountDto("user@test.com", "Str0ng@Pass!", "Test User", Gender.F, DateOnly.FromDateTime(DateTime.Now));
        _accountService.CreateAccountAsync(createDto, Arg.Any<CancellationToken>()).Returns(DefaultAccountDto);
        var httpContext = NewHttpContext(nativeClient: true);

        var result = await AccountEndpoints.Create(createDto, _accountService, httpContext, CancellationToken.None);

        var created = result.Should().BeOfType<Created<AccountTokenResponseDto>>().Subject;
        created.Value.Should().NotBeNull();
        created.Value!.SessionToken.Should().Be("access-token");
        created.Value!.RefreshToken.Should().Be("refresh-token");

        var setCookieHeader = httpContext.Response.Headers.SetCookie.ToString();
        setCookieHeader.Should().Contain("sessionToken=access-token");
        setCookieHeader.Should().Contain("refreshToken=refresh-token");
    }

    [Fact(DisplayName = "Login: browser client (no header) gets AccountResponseDto, no tokens in body")]
    public async Task Login_BrowserClient_ReturnsProfileOnlyBody()
    {
        var loginDto = new LoginAccountDto("user@test.com", "Str0ng@Pass!");
        _accountService.LoginAccountAsync(loginDto, Arg.Any<CancellationToken>()).Returns(DefaultAccountDto);
        var httpContext = NewHttpContext(nativeClient: false);

        var result = await AccountEndpoints.Login(loginDto, _accountService, httpContext, CancellationToken.None);

        var ok = result.Should().BeOfType<Ok<AccountResponseDto>>().Subject;
        ok.Value.Should().NotBeNull();

        typeof(AccountResponseDto).GetProperty("SessionToken").Should().BeNull();
        typeof(AccountResponseDto).GetProperty("RefreshToken").Should().BeNull();
    }

    [Fact(DisplayName = "Login: native client (X-Client-Type: mobile) gets both tokens in body")]
    public async Task Login_NativeClient_ReturnsTokensInBody()
    {
        var loginDto = new LoginAccountDto("user@test.com", "Str0ng@Pass!");
        _accountService.LoginAccountAsync(loginDto, Arg.Any<CancellationToken>()).Returns(DefaultAccountDto);
        var httpContext = NewHttpContext(nativeClient: true);

        var result = await AccountEndpoints.Login(loginDto, _accountService, httpContext, CancellationToken.None);

        var ok = result.Should().BeOfType<Ok<AccountTokenResponseDto>>().Subject;
        ok.Value.Should().NotBeNull();
        ok.Value!.SessionToken.Should().Be("access-token");
        ok.Value!.RefreshToken.Should().Be("refresh-token");
    }

    [Fact(DisplayName = "Refresh: browser client without cookie is unauthorized, body token ignored")]
    public async Task Refresh_BrowserClientNoCookie_ReturnsUnauthorized()
    {
        var refreshDto = new RefreshTokenDto("body-token", "user@test.com");
        var httpContext = NewHttpContext(nativeClient: false);

        var result = await AccountEndpoints.Refresh(refreshDto, _accountService, httpContext, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedHttpResult>();
        await _accountService.DidNotReceive().RefreshTokenAsync(Arg.Any<RefreshTokenDto>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Refresh: browser client uses cookie token and gets AccountResponseDto")]
    public async Task Refresh_BrowserClientWithCookie_UsesCookieTokenAndReturnsProfileOnly()
    {
        var refreshDto = new RefreshTokenDto("body-token-should-be-ignored", "user@test.com");
        _accountService.RefreshTokenAsync(Arg.Any<RefreshTokenDto>(), Arg.Any<CancellationToken>()).Returns(DefaultAccountDto);
        var httpContext = NewHttpContext(nativeClient: false);
        httpContext.Request.Headers.Cookie = "refreshToken=cookie-token";

        var result = await AccountEndpoints.Refresh(refreshDto, _accountService, httpContext, CancellationToken.None);

        result.Should().BeOfType<Ok<AccountResponseDto>>();
        await _accountService.Received(1).RefreshTokenAsync(
            Arg.Is<RefreshTokenDto>(d => d.RefreshToken == "cookie-token"),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Refresh: native client sends refresh token via body without a cookie and rotates it")]
    public async Task Refresh_NativeClientBodyToken_NoCookieRequired_RotatesAndReturnsNewPair()
    {
        var rotatedAccountDto = DefaultAccountDto with { SessionToken = "new-access-token", RefreshToken = "new-refresh-token" };
        var refreshDto = new RefreshTokenDto("old-native-refresh-token", "user@test.com");
        _accountService.RefreshTokenAsync(Arg.Any<RefreshTokenDto>(), Arg.Any<CancellationToken>()).Returns(rotatedAccountDto);
        var httpContext = NewHttpContext(nativeClient: true);

        var result = await AccountEndpoints.Refresh(refreshDto, _accountService, httpContext, CancellationToken.None);

        await _accountService.Received(1).RefreshTokenAsync(
            Arg.Is<RefreshTokenDto>(d => d.RefreshToken == "old-native-refresh-token" && d.Email == "user@test.com"),
            Arg.Any<CancellationToken>());

        var ok = result.Should().BeOfType<Ok<AccountTokenResponseDto>>().Subject;
        ok.Value.Should().NotBeNull();
        ok.Value!.SessionToken.Should().Be("new-access-token");
        ok.Value!.RefreshToken.Should().Be("new-refresh-token");

        var setCookieHeader = httpContext.Response.Headers.SetCookie.ToString();
        setCookieHeader.Should().Contain("sessionToken=new-access-token");
        setCookieHeader.Should().Contain("refreshToken=new-refresh-token");
    }

    [Fact(DisplayName = "Refresh: native client missing body token is unauthorized even without a cookie")]
    public async Task Refresh_NativeClientMissingBodyToken_ReturnsUnauthorized()
    {
        var refreshDto = new RefreshTokenDto(string.Empty, "user@test.com");
        var httpContext = NewHttpContext(nativeClient: true);

        var result = await AccountEndpoints.Refresh(refreshDto, _accountService, httpContext, CancellationToken.None);

        result.Should().BeOfType<UnauthorizedHttpResult>();
        await _accountService.DidNotReceive().RefreshTokenAsync(Arg.Any<RefreshTokenDto>(), Arg.Any<CancellationToken>());
    }
}
