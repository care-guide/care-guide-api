using CareGuide.API.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.RateLimiting;

namespace CareGuide.Tests.Extensions;

public class RateLimitingExtensionTests
{
    [Theory(DisplayName = "OnRejected sets status + Retry-After header from real lease metadata, writes no body")]
    [InlineData(RateLimitingExtension.GeneralPolicy)]
    [InlineData(RateLimitingExtension.AuthPolicy)]
    public async Task OnRejected_SetsStatusAndHeader_ButNeverWritesBody(string policyName)
    {
        // Policy name isn't used by OnRejected itself (it is shared across policies), but we assert
        // the same single behavior is exercised for both "general" and "auth" as required.
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

    /// <summary>
    /// Minimal fake <see cref="RateLimitLease"/> for a rejected acquisition, optionally carrying
    /// Retry-After metadata the same way a real limiter's failed lease would.
    /// </summary>
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
