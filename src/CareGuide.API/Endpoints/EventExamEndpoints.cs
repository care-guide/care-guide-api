using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.EventExam;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class EventExamEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/exam")
                .WithTags("Event Exams")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetByEventId)
                 .WithName("GetEventExam")
                 .WithSummary("Get event exam")
                 .WithDescription("Retrieves the exam details for a specific event.")
                 .Produces<EventExamDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateEventExam")
                 .WithSummary("Create event exam")
                 .WithDescription("Creates the exam details for a specific event.")
                 .Accepts<CreateEventExamDto>("application/json")
                 .Produces<EventExamDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/", Update)
                 .WithName("UpdateEventExam")
                 .WithSummary("Update event exam")
                 .WithDescription("Updates the exam details for a specific event.")
                 .Accepts<UpdateEventExamDto>("application/json")
                 .Produces<EventExamDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/", Delete)
                 .WithName("DeleteEventExam")
                 .WithSummary("Delete event exam")
                 .WithDescription("Deletes the exam details for a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetByEventId(Guid eventId, [FromServices] IEventExamService eventExamService, CancellationToken cancellationToken)
        {
            var result = await eventExamService.GetByEventIdAsync(eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(Guid eventId, [FromBody] CreateEventExamDto createEventExamDto, [FromServices] IEventExamService eventExamService, CancellationToken cancellationToken)
        {
            var created = await eventExamService.CreateAsync(eventId, createEventExamDto, cancellationToken);
            return Results.Created($"/events/{eventId}/exam", created);
        }

        private static async Task<IResult> Update(Guid eventId, [FromBody] UpdateEventExamDto updateEventExamDto, [FromServices] IEventExamService eventExamService, CancellationToken cancellationToken)
        {
            var result = await eventExamService.UpdateAsync(eventId, updateEventExamDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Delete(Guid eventId, [FromServices] IEventExamService eventExamService, CancellationToken cancellationToken)
        {
            await eventExamService.DeleteAsync(eventId, cancellationToken);
            return Results.NoContent();
        }
    }
}
