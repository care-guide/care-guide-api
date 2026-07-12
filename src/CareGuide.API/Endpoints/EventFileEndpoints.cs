using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventFile;
using CareGuide.Models.DTOs.Shared;

namespace CareGuide.API.Endpoints
{
    public class EventFileEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/files")
                .WithTags("Event Files")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAllByEventId)
                 .WithName("GetEventFiles")
                 .WithSummary("Get event files")
                 .WithDescription("Retrieves all files attached to a specific event using pagination parameters.")
                 .Produces<PagedResult<EventFileDto>>(StatusCodes.Status200OK);

            group.MapPost("/{fileId:guid}", Attach)
                 .WithName("AttachEventFile")
                 .WithSummary("Attach file to event")
                 .WithDescription("Attaches an existing file to a specific event.")
                 .Produces<EventFileDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/{fileId:guid}", Detach)
                 .WithName("DetachEventFile")
                 .WithSummary("Detach file from event")
                 .WithDescription("Detaches a file from a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/{fileId:guid}/confirm", ConfirmUpload)
                 .WithName("ConfirmEventFileUpload")
                 .WithSummary("Confirm an uploaded event file")
                 .WithDescription("Validates the private object metadata, marks the file as uploaded, and attaches it to the event atomically.")
                 .Produces<EventFileDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapGet("/{fileId:guid}/download", CreateDownload)
                 .WithName("CreateEventFileDownload")
                 .WithSummary("Create a temporary event-file download")
                 .WithDescription("Authorizes access to an attached private file and returns a short-lived pre-signed GET URL.")
                 .Produces<EventFileDownloadDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetAllByEventId(Guid eventId, int page, int pageSize, IEventFileService eventFileService, CancellationToken cancellationToken)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await eventFileService.GetAllByEventIdAsync(page, pageSize, eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Attach(Guid eventId, Guid fileId, IEventFileService eventFileService, CancellationToken cancellationToken)
        {
            var created = await eventFileService.AttachAsync(eventId, fileId, cancellationToken);
            return Results.Created($"/events/{eventId}/files/{fileId}", created);
        }

        private static async Task<IResult> Detach(Guid eventId, Guid fileId, IEventFileService eventFileService, CancellationToken cancellationToken)
        {
            await eventFileService.DetachAsync(eventId, fileId, cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> ConfirmUpload(Guid eventId, Guid fileId, IEventFileService eventFileService, CancellationToken cancellationToken)
        {
            var result = await eventFileService.ConfirmUploadAsync(eventId, fileId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> CreateDownload(
            Guid eventId,
            Guid fileId,
            IEventFileDownloadService downloadService,
            CancellationToken cancellationToken)
        {
            var result = await downloadService.CreateDownloadAsync(eventId, fileId, cancellationToken);
            return Results.Ok(result);
        }
    }
}
