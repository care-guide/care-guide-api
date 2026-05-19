using Microsoft.AspNetCore.Authorization;

namespace CareGuide.API.Middlewares
{
    public class SessionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();

            if (endpoint?.Metadata.GetMetadata<IgnoreSessionMiddleware>() is not null ||
                endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            {
                await next(context);
                return;
            }

            var path = context.Request.Path.Value?.ToLowerInvariant();
            if (path is not null &&
                (path.StartsWith("/scalar") ||
                 path.StartsWith("/api-reference") ||
                 path.StartsWith("/api-docs") ||
                 path.StartsWith("/openapi")))
            {
                await next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Invalid or missing token.");

            await next(context);
        }
    }
}
