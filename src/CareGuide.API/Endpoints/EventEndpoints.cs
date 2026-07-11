using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Event;
using CareGuide.Models.DTOs.FileAsset;
using CareGuide.Models.Enums;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class EventEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events")
                .WithTags("Events")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAll)
                 .WithName("GetAllEvents")
                 .WithSummary("Get all events")
                 .WithDescription("Retrieves all events for the authenticated person using pagination parameters. " +
                    "Optionally filters by `startDate` using the inclusive `from`/`to` range and orders results by `startDate` " +
                    "('Asc' by default, or 'Desc').")
                 .Produces<List<EventDto>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetEventById")
                 .WithSummary("Get event by id")
                 .WithDescription("Retrieves a specific event for the authenticated person by its identifier.")
                 .Produces<EventDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateEvent")
                 .WithSummary("Create event")
                 .WithDescription("Creates a new event for the authenticated person.")
                 .Accepts<CreateEventDto>("application/json")
                 .Produces<EventDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPost("/{id:guid}/files/uploads", InitiateFileUpload)
                 .WithName("InitiateEventFileUpload")
                 .WithSummary("Initiate a direct event file upload")
                 .WithDescription("Creates a pending private file and returns a short-lived pre-signed PUT URL.")
                 .Accepts<InitiateEventFileUploadDto>("application/json")
                 .Produces<InitiateEventFileUploadResponseDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPut("/{id:guid}", Update)
                 .WithName("UpdateEvent")
                 .WithSummary("Update event")
                 .WithDescription("Updates an existing event for the authenticated person by its identifier.")
                 .Accepts<UpdateEventDto>("application/json")
                 .Produces<EventDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/person", DeleteAll)
                 .WithName("DeleteAllEvents")
                 .WithSummary("Delete all events")
                 .WithDescription("Deletes all events associated with the authenticated person.")
                 .Produces(StatusCodes.Status204NoContent);

            group.MapDelete("/", DeleteByIds)
                 .WithName("DeleteEventsByIds")
                 .WithSummary("Delete multiple events")
                 .WithDescription("Deletes multiple events for the authenticated person using a list of event identifiers provided in the request body.")
                 .Accepts<List<Guid>>("application/json")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> GetAll(
            int page,
            int pageSize,
            IEventService eventService,
            CancellationToken cancellationToken,
            DateTime? from = null,
            DateTime? to = null,
            EventSortOrder sort = EventSortOrder.Asc)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await eventService.GetAllByPersonAsync(from, to, sort, page, pageSize, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(Guid id, IEventService eventService, CancellationToken cancellationToken)
        {
            var result = await eventService.GetAsync(id, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(CreateEventDto createEventDto, IEventService eventService, CancellationToken cancellationToken)
        {
            var created = await eventService.CreateAsync(createEventDto, cancellationToken);
            return Results.Created($"/events/{created.Id}", created);
        }

        private static async Task<IResult> Update(Guid id, UpdateEventDto updateEventDto, IEventService eventService, CancellationToken cancellationToken)
        {
            var result = await eventService.UpdateAsync(id, updateEventDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> InitiateFileUpload(
            Guid id,
            InitiateEventFileUploadDto dto,
            IEventFileUploadService uploadService,
            CancellationToken cancellationToken)
        {
            var result = await uploadService.InitiateAsync(id, dto, cancellationToken);
            return Results.Created($"/events/{id}/files/{result.FileId}", result);
        }

        private static async Task<IResult> DeleteAll(IEventService eventService, CancellationToken cancellationToken)
        {
            await eventService.DeleteAllByPersonAsync(cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IEventService eventService, CancellationToken cancellationToken)
        {
            await eventService.DeleteByIdsAsync(ids, cancellationToken);
            return Results.NoContent();
        }
    }
}
