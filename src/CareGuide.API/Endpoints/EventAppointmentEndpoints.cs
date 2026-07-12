using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.EventAppointment;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class EventAppointmentEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/appointment")
                .WithTags("Event Appointments")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetByEventId)
                 .WithName("GetEventAppointment")
                 .WithSummary("Get event appointment")
                 .WithDescription("Retrieves the appointment details for a specific event.")
                 .Produces<EventAppointmentDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateEventAppointment")
                 .WithSummary("Create event appointment")
                 .WithDescription("Creates the appointment details for a specific event.")
                 .Accepts<CreateEventAppointmentDto>("application/json")
                 .Produces<EventAppointmentDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/", Update)
                 .WithName("UpdateEventAppointment")
                 .WithSummary("Update event appointment")
                 .WithDescription("Updates the appointment details for a specific event.")
                 .Accepts<UpdateEventAppointmentDto>("application/json")
                 .Produces<EventAppointmentDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/", Delete)
                 .WithName("DeleteEventAppointment")
                 .WithSummary("Delete event appointment")
                 .WithDescription("Deletes the appointment details for a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetByEventId(Guid eventId, [FromServices] IEventAppointmentService eventAppointmentService, CancellationToken cancellationToken)
        {
            var result = await eventAppointmentService.GetByEventIdAsync(eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(Guid eventId, [FromBody] CreateEventAppointmentDto createEventAppointmentDto, [FromServices] IEventAppointmentService eventAppointmentService, CancellationToken cancellationToken)
        {
            var created = await eventAppointmentService.CreateAsync(eventId, createEventAppointmentDto, cancellationToken);
            return Results.Created($"/api/v1/events/{eventId}/appointment", created);
        }

        private static async Task<IResult> Update(Guid eventId, [FromBody] UpdateEventAppointmentDto updateEventAppointmentDto, [FromServices] IEventAppointmentService eventAppointmentService, CancellationToken cancellationToken)
        {
            var result = await eventAppointmentService.UpdateAsync(eventId, updateEventAppointmentDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Delete(Guid eventId, [FromServices] IEventAppointmentService eventAppointmentService, CancellationToken cancellationToken)
        {
            await eventAppointmentService.DeleteAsync(eventId, cancellationToken);
            return Results.NoContent();
        }
    }
}
