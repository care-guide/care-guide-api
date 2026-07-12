using System.Security.Claims;
using CareGuide.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace CareGuide.Tests.Extensions;

public class RateLimitingExtensionTests
{
    private const string SharedCgnatIp = "203.0.113.10";

    [Theory(DisplayName = "OnRejected sets status + Retry-After header from real lease metadata, writes no body")]
    [InlineData(RateLimitingExtension.GeneralPolicy)]
    [InlineData(RateLimitingExtension.AuthPolicy)]
    public async Task OnRejected_SetsStatusAndHeader_ButNeverWritesBody(string policyName)
    {
        _ = policyName;

        var services = new ServiceCollection();
        services.AddApiRateLimiting();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<RateLimiterOptions>>().Value;
        options.OnRejected.Should().NotBeNull();

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var retryAfter = TimeSpan.FromSeconds(42);
        var lease = new FakeRejectedLease(retryAfter);

        var rejectedContext = new OnRejectedContext
        {
            HttpContext = context,
            Lease = lease
        };

        await options.OnRejected!(rejectedContext, CancellationToken.None);

        context.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        context.Response.Headers.RetryAfter.ToString().Should().Be("42");

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        var body = await reader.ReadToEndAsync();
        body.Should().BeEmpty("OnRejected must not write the response body; ErrorHandlerMiddleware is the single writer");
    }

    [Fact(DisplayName = "OnRejected sets status even when the lease has no Retry-After metadata")]
    public async Task OnRejected_NoRetryAfterMetadata_StillSetsStatusAndNoBody()
    {
        var services = new ServiceCollection();
        services.AddApiRateLimiting();
        using var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<RateLimiterOptions>>().Value;

        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var lease = new FakeRejectedLease(retryAfter: null);

        var rejectedContext = new OnRejectedContext
        {
            HttpContext = context,
            Lease = lease
        };

        await options.OnRejected!(rejectedContext, CancellationToken.None);

        context.Response.StatusCode.Should().Be(StatusCodes.Status429TooManyRequests);
        context.Response.Headers.RetryAfter.ToString().Should().BeEmpty();

        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        (await reader.ReadToEndAsync()).Should().BeEmpty();
    }

    [Fact]
    public void ResolveIdentityPartitionKey_TwoAuthenticatedUsersBehindSameIp_ReturnDifferentKeys()
    {
        var userAContext = CreateAuthenticatedContext(userId: "11111111-1111-1111-1111-111111111111", ip: SharedCgnatIp);
        var userBContext = CreateAuthenticatedContext(userId: "22222222-2222-2222-2222-222222222222", ip: SharedCgnatIp);

        var keyA = RateLimitingExtension.ResolveIdentityPartitionKey(userAContext);
        var keyB = RateLimitingExtension.ResolveIdentityPartitionKey(userBContext);

        keyA.Should().NotBe(keyB);
        keyA.Should().Be("user:11111111-1111-1111-1111-111111111111");
        keyB.Should().Be("user:22222222-2222-2222-2222-222222222222");
    }

    [Fact]
    public void ResolveIdentityPartitionKey_SameAuthenticatedUserAcrossRequests_ReturnsSameKey()
    {
        var firstRequest = CreateAuthenticatedContext(userId: "33333333-3333-3333-3333-333333333333", ip: "198.51.100.1");
        var secondRequest = CreateAuthenticatedContext(userId: "33333333-3333-3333-3333-333333333333", ip: "198.51.100.2");

        var keyFirst = RateLimitingExtension.ResolveIdentityPartitionKey(firstRequest);
        var keySecond = RateLimitingExtension.ResolveIdentityPartitionKey(secondRequest);

        keyFirst.Should().Be(keySecond);
    }

    [Fact]
    public void ResolveIdentityPartitionKey_AnonymousRequest_FallsBackToIpPartitioning()
    {
        var context = CreateAnonymousContext(ip: SharedCgnatIp);

        var key = RateLimitingExtension.ResolveIdentityPartitionKey(context);

        key.Should().Be($"ip:{SharedCgnatIp}");
    }

    [Fact]
    public void ResolveIdentityPartitionKey_AnonymousRequest_NeverUsesSpoofableHeaderAsIdentity()
    {
        var context = CreateAnonymousContext(ip: SharedCgnatIp);
        context.Request.Headers["X-User-Id"] = "attacker-supplied-id";

        var key = RateLimitingExtension.ResolveIdentityPartitionKey(context);

        key.Should().Be($"ip:{SharedCgnatIp}");
        key.Should().NotContain("attacker-supplied-id");
    }

    [Fact]
    public void ResolveClientIp_UsesFirstAddressFromXForwardedFor()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = "203.0.113.55, 10.0.0.1";

        var ip = RateLimitingExtension.ResolveClientIp(context);

        ip.Should().Be("203.0.113.55");
    }

    [Fact]
    public void ResolveClientIp_TwoAnonymousUsersBehindSameIp_ShareTheSameKey()
    {
        var contextA = CreateAnonymousContext(ip: SharedCgnatIp);
        var contextB = CreateAnonymousContext(ip: SharedCgnatIp);

        var ipA = RateLimitingExtension.ResolveClientIp(contextA);
        var ipB = RateLimitingExtension.ResolveClientIp(contextB);

        ipA.Should().Be(ipB);
    }

    private static DefaultHttpContext CreateAuthenticatedContext(string userId, string ip)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = ip;

        var identity = new ClaimsIdentity(
        [
            new Claim("sub", userId)
        ], authenticationType: "Bearer");

        context.User = new ClaimsPrincipal(identity);
        return context;
    }

    private static DefaultHttpContext CreateAnonymousContext(string ip)
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Forwarded-For"] = ip;
        context.User = new ClaimsPrincipal(new ClaimsIdentity());
        return context;
    }

    private sealed class FakeRejectedLease : RateLimitLease
    {
        private readonly TimeSpan? _retryAfter;

        public FakeRejectedLease(TimeSpan? retryAfter)
        {
            _retryAfter = retryAfter;
        }

        public override bool IsAcquired => false;

        public override IEnumerable<string> MetadataNames =>
            _retryAfter.HasValue ? new[] { MetadataName.RetryAfter.Name } : Array.Empty<string>();

        public override bool TryGetMetadata(string metadataName, out object? metadata)
        {
            if (metadataName == MetadataName.RetryAfter.Name && _retryAfter.HasValue)
            {
                metadata = _retryAfter.Value;
                return true;
            }

            metadata = null;
            return false;
        }
    }
}
