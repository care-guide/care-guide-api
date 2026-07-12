using CareGuide.API.Helpers;
using CareGuide.Models.Constants;
using Microsoft.AspNetCore.Http;

namespace CareGuide.Tests.Helpers;

public class AuthCookieHelperTests
{
    [Fact(DisplayName = "IsNativeClient: no client-type header defaults to browser")]
    public void IsNativeClient_NoHeader_ReturnsFalse()
    {
        var context = new DefaultHttpContext();

        var result = AuthCookieHelper.IsNativeClient(context.Request);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "IsNativeClient: X-Client-Type: mobile identifies a native client")]
    public void IsNativeClient_MobileHeader_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[AuthClientConstants.ClientTypeHeaderName] = AuthClientConstants.MobileClientTypeValue;

        var result = AuthCookieHelper.IsNativeClient(context.Request);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "IsNativeClient: header value comparison is case-insensitive")]
    public void IsNativeClient_MobileHeaderDifferentCase_ReturnsTrue()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[AuthClientConstants.ClientTypeHeaderName] = "MOBILE";

        var result = AuthCookieHelper.IsNativeClient(context.Request);

        result.Should().BeTrue();
    }

    [Fact(DisplayName = "IsNativeClient: unrecognized header value defaults to browser")]
    public void IsNativeClient_UnknownHeaderValue_ReturnsFalse()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers[AuthClientConstants.ClientTypeHeaderName] = "web";

        var result = AuthCookieHelper.IsNativeClient(context.Request);

        result.Should().BeFalse();
    }

    [Fact(DisplayName = "AppendRefreshToken: sets HttpOnly, Secure, SameSite=Strict cookie")]
    public void AppendRefreshToken_ValidToken_SetsSecureCookie()
    {
        var context = new DefaultHttpContext();

        AuthCookieHelper.AppendRefreshToken(context.Response, "refresh-token-value");

        var setCookieHeader = context.Response.Headers.SetCookie.ToString();
        setCookieHeader.Should().Contain("refreshToken=refresh-token-value");
        setCookieHeader.Should().Contain("httponly");
        setCookieHeader.Should().Contain("secure");
        setCookieHeader.Should().Contain("samesite=strict");
    }

    [Fact(DisplayName = "AppendSessionToken: sets HttpOnly, Secure, SameSite=Strict cookie")]
    public void AppendSessionToken_ValidToken_SetsSecureCookie()
    {
        var context = new DefaultHttpContext();

        AuthCookieHelper.AppendSessionToken(context.Response, "session-token-value");

        var setCookieHeader = context.Response.Headers.SetCookie.ToString();
        setCookieHeader.Should().Contain("sessionToken=session-token-value");
        setCookieHeader.Should().Contain("httponly");
        setCookieHeader.Should().Contain("secure");
        setCookieHeader.Should().Contain("samesite=strict");
    }
}
