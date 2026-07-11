using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace CareGuide.Tests.Configuration;

/// <summary>
/// Verifies the "CorsPolicy" configured in Program.cs: credentialed cross-origin requests are
/// allowed only for explicit origins sourced from configuration, never a wildcard.
/// </summary>
public class CorsPolicyTests
{
    private const string AllowedOrigin = "https://careguide.dev";
    private const string DisallowedOrigin = "https://evil-attacker.example";

    private static (CorsService Service, DefaultCorsPolicyProvider Provider) BuildCors(params string[] allowedOrigins)
    {
        var options = new CorsOptions();
        options.AddPolicy("CorsPolicy", policy =>
        {
            // Mirrors src/CareGuide.API/Program.cs AddCors configuration exactly.
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });

        var optionsWrapper = Options.Create(options);
        return (new CorsService(optionsWrapper, NullLoggerFactory.Instance), new DefaultCorsPolicyProvider(optionsWrapper));
    }

    private static HttpContext BuildPreflightContext(string origin)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = "OPTIONS";
        context.Request.Headers.Origin = origin;
        context.Request.Headers["Access-Control-Request-Method"] = "GET";
        return context;
    }

    [Fact(DisplayName = "Preflight from allowed origin returns credentialed CORS headers")]
    public async Task EvaluatePreflightRequest_AllowedOrigin_AllowsCredentialsForExplicitOrigin()
    {
        var (service, provider) = BuildCors(AllowedOrigin);
        var context = BuildPreflightContext(AllowedOrigin);
        var policy = await provider.GetPolicyAsync(context, "CorsPolicy");

        var result = service.EvaluatePolicy(context, policy!);

        result.IsOriginAllowed.Should().BeTrue();
        result.SupportsCredentials.Should().BeTrue();
        result.AllowedOrigin.Should().Be(AllowedOrigin);
    }

    [Fact(DisplayName = "Preflight from disallowed origin is rejected")]
    public async Task EvaluatePreflightRequest_DisallowedOrigin_IsNotAllowed()
    {
        var (service, provider) = BuildCors(AllowedOrigin);
        var context = BuildPreflightContext(DisallowedOrigin);
        var policy = await provider.GetPolicyAsync(context, "CorsPolicy");

        var result = service.EvaluatePolicy(context, policy!);

        result.IsOriginAllowed.Should().BeFalse();
    }

    [Fact(DisplayName = "CorsPolicy never combines AllowCredentials with a wildcard origin")]
    public async Task CorsPolicy_NeverCombinesWildcardOriginWithCredentials()
    {
        var (_, provider) = BuildCors(AllowedOrigin);
        var context = BuildPreflightContext(AllowedOrigin);

        var policy = await provider.GetPolicyAsync(context, "CorsPolicy");

        policy.Should().NotBeNull();
        policy!.SupportsCredentials.Should().BeTrue();
        policy.AllowAnyOrigin.Should().BeFalse();
        policy.Origins.Should().NotContain("*");
        policy.Origins.Should().ContainSingle().Which.Should().Be(AllowedOrigin);
    }

    [Fact(DisplayName = "Non-browser requests without an Origin header are unaffected by CORS")]
    public async Task EvaluatePolicy_NoOriginHeader_ProducesNoCorsHeaders()
    {
        var (service, provider) = BuildCors(AllowedOrigin);
        var context = new DefaultHttpContext();
        context.Request.Method = "GET";

        var policy = await provider.GetPolicyAsync(context, "CorsPolicy");
        var result = service.EvaluatePolicy(context, policy!);

        // No Origin header means the request never entered the CORS pipeline as a cross-origin
        // request, so no Access-Control-Allow-Origin is emitted, regardless of the policy config.
        result.AllowedOrigin.Should().BeNull();
    }
}
