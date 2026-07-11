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
                // General: sliding window, 60 req/min per authenticated user (falls back to IP if anonymous)
                options.AddPolicy(GeneralPolicy, httpContext =>
                    RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: ResolveIdentityPartitionKey(httpContext),
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

                options.OnRejected = async (context, cancellationToken) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter =
                            ((int)retryAfter.TotalSeconds).ToString();
                    }

                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", cancellationToken);
                };
            });

            return services;
        }

        /// <summary>
        /// Partition key for the <see cref="GeneralPolicy"/>. Authenticated requests partition by the
        /// validated JWT "sub" (userId) claim so users sharing a public IP (e.g. mobile carrier CGNAT)
        /// don't throttle each other. The claim comes from the validated token/principal only — never
        /// from a client-supplied header, which would be trivially spoofable. Genuinely anonymous
        /// requests fall back to IP-based partitioning.
        /// </summary>
        internal static string ResolveIdentityPartitionKey(HttpContext context)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var userId = user.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
                if (!string.IsNullOrWhiteSpace(userId))
                    return $"user:{userId}";
            }

            return $"ip:{ResolveClientIp(context)}";
        }

        internal static string ResolveClientIp(HttpContext context)
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
