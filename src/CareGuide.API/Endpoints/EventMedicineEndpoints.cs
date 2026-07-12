using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.EventMedicine;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class EventMedicineEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/medicine")
                .WithTags("Event Medicines")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetByEventId)
                 .WithName("GetEventMedicine")
                 .WithSummary("Get event medicine")
                 .WithDescription("Retrieves the medicine details for a specific event.")
                 .Produces<EventMedicineDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateEventMedicine")
                 .WithSummary("Create event medicine")
                 .WithDescription("Creates the medicine details for a specific event.")
                 .Accepts<CreateEventMedicineDto>("application/json")
                 .Produces<EventMedicineDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/", Update)
                 .WithName("UpdateEventMedicine")
                 .WithSummary("Update event medicine")
                 .WithDescription("Updates the medicine details for a specific event.")
                 .Accepts<UpdateEventMedicineDto>("application/json")
                 .Produces<EventMedicineDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/", Delete)
                 .WithName("DeleteEventMedicine")
                 .WithSummary("Delete event medicine")
                 .WithDescription("Deletes the medicine details for a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetByEventId(Guid eventId, [FromServices] IEventMedicineService eventMedicineService, CancellationToken cancellationToken)
        {
            var result = await eventMedicineService.GetByEventIdAsync(eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(Guid eventId, [FromBody] CreateEventMedicineDto createEventMedicineDto, [FromServices] IEventMedicineService eventMedicineService, CancellationToken cancellationToken)
        {
            var created = await eventMedicineService.CreateAsync(eventId, createEventMedicineDto, cancellationToken);
            return Results.Created($"/api/v1/events/{eventId}/medicine", created);
        }

        private static async Task<IResult> Update(Guid eventId, [FromBody] UpdateEventMedicineDto updateEventMedicineDto, [FromServices] IEventMedicineService eventMedicineService, CancellationToken cancellationToken)
        {
            var result = await eventMedicineService.UpdateAsync(eventId, updateEventMedicineDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Delete(Guid eventId, [FromServices] IEventMedicineService eventMedicineService, CancellationToken cancellationToken)
        {
            await eventMedicineService.DeleteAsync(eventId, cancellationToken);
            return Results.NoContent();
        }
    }
}
