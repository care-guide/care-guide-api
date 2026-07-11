using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;

namespace CareGuide.API.Endpoints;

public sealed class FileAssetEndpoints : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGroup("/files")
            .WithTags("Files")
            .WithDefaultProblemResponses()
            .MapDelete("/{fileId:guid}", Delete)
            .WithName("DeleteFileAsset")
            .WithSummary("Permanently delete an unreferenced private file")
            .WithDescription("Deletes an owned file only when no event references remain. Partial failures remain retryable.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> Delete(Guid fileId, IFileLifecycleService service, CancellationToken cancellationToken)
    {
        await service.DeleteAsync(fileId, cancellationToken);
        return Results.NoContent();
    }
}
