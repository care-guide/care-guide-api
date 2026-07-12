using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.EventSurgery;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class EventSurgeryEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/surgery")
                .WithTags("Event Surgeries")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetByEventId)
                 .WithName("GetEventSurgery")
                 .WithSummary("Get event surgery")
                 .WithDescription("Retrieves the surgery details for a specific event.")
                 .Produces<EventSurgeryDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateEventSurgery")
                 .WithSummary("Create event surgery")
                 .WithDescription("Creates the surgery details for a specific event.")
                 .Accepts<CreateEventSurgeryDto>("application/json")
                 .Produces<EventSurgeryDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/", Update)
                 .WithName("UpdateEventSurgery")
                 .WithSummary("Update event surgery")
                 .WithDescription("Updates the surgery details for a specific event.")
                 .Accepts<UpdateEventSurgeryDto>("application/json")
                 .Produces<EventSurgeryDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/", Delete)
                 .WithName("DeleteEventSurgery")
                 .WithSummary("Delete event surgery")
                 .WithDescription("Deletes the surgery details for a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetByEventId(Guid eventId, [FromServices] IEventSurgeryService eventSurgeryService, CancellationToken cancellationToken)
        {
            var result = await eventSurgeryService.GetByEventIdAsync(eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(Guid eventId, [FromBody] CreateEventSurgeryDto createEventSurgeryDto, [FromServices] IEventSurgeryService eventSurgeryService, CancellationToken cancellationToken)
        {
            var created = await eventSurgeryService.CreateAsync(eventId, createEventSurgeryDto, cancellationToken);
            return Results.Created($"/api/v1/events/{eventId}/surgery", created);
        }

        private static async Task<IResult> Update(Guid eventId, [FromBody] UpdateEventSurgeryDto updateEventSurgeryDto, [FromServices] IEventSurgeryService eventSurgeryService, CancellationToken cancellationToken)
        {
            var result = await eventSurgeryService.UpdateAsync(eventId, updateEventSurgeryDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Delete(Guid eventId, [FromServices] IEventSurgeryService eventSurgeryService, CancellationToken cancellationToken)
        {
            await eventSurgeryService.DeleteAsync(eventId, cancellationToken);
            return Results.NoContent();
        }
    }
}
