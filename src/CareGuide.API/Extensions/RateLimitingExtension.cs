using System.Net;
using System.Threading.RateLimiting;

namespace CareGuide.API.Extensions
{
    public static class RateLimitingExtension
    {
        public const string AuthPolicy = "auth";
        public const string GeneralPolicy = "general";

        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // General: sliding window, 60 req/min per IP
                options.AddPolicy(GeneralPolicy, httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: ResolveClientIp(httpContext),
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 60,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 6,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                // Auth: stricter fixed window — brute-force protection
                options.AddPolicy(AuthPolicy, httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: ResolveClientIp(httpContext),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(1),
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0
                        }));

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // Only status code + Retry-After header are set here. The response BODY is
                // intentionally NOT written in this callback: ErrorHandlerMiddleware wraps the
                // rate limiter in the pipeline and is the single writer of the 429 body
                // (as application/problem+json). Writing a body here too would race with the
                // middleware's write and risk an "response already started" conflict.
                options.OnRejected = (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString();
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    return ValueTask.CompletedTask;
                };
            });

            return services;
        }

        private static string ResolveClientIp(HttpContext context)
        {
            var forwarded = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                var ip = forwarded.Split(',')[0].Trim();
                if (IPAddress.TryParse(ip, out _))
                    return ip;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
