using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Middlewares;
using CareGuide.Models.DTOs.HealthCheck;

namespace CareGuide.API.Endpoints;

/// <summary>
/// Anonymous liveness probe. Mapped directly on the root app (see Program.cs) so it sits outside
/// the authenticated "/api/v1" group and is reachable without a session — suitable for
/// container/orchestrator startup and liveness checks.
///
/// Intentionally returns nothing but a fixed status literal: no build info, no connection strings,
/// no internal ids, no dependency checks (DB, storage, etc). The previous authenticated
/// "GET /api/v1/health-check" endpoint, which echoed the caller's session Guid, has been removed
/// to avoid leaking session identifiers on what should be a trivial liveness signal.
/// </summary>
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
