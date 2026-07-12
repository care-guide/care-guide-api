using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Middlewares;
using CareGuide.Models.DTOs.HealthCheck;

namespace CareGuide.API.Endpoints;

public class HealthCheckEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/health")
            .WithTags("Health Checks");

        group.MapGet("/", GetLiveness)
             .AllowAnonymous()
             .WithMetadata(new IgnoreSessionMiddleware())
             .WithName("HealthLiveness")
             .WithSummary("Liveness probe")
             .WithDescription("Anonymous liveness check. Returns a fixed status payload only; no auth, no dependency checks, no internal identifiers.")
             .Produces<HealthStatusDto>(StatusCodes.Status200OK);

        group.MapGet("/live", GetLiveness)
             .AllowAnonymous()
             .WithMetadata(new IgnoreSessionMiddleware())
             .WithName("HealthLive")
             .WithSummary("Liveness probe")
             .WithDescription("Anonymous liveness check. Returns a fixed status payload only; no auth, no dependency checks, no internal identifiers.")
             .Produces<HealthStatusDto>(StatusCodes.Status200OK);
    }

    private static IResult GetLiveness() => Results.Ok(new HealthStatusDto("ok"));
}
