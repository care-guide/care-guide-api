using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.DTOs.EventAddress;

namespace CareGuide.API.Endpoints
{
    public class EventAddressEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/events/{eventId:guid}/addresses")
                .WithTags("Event Addresses")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAllByEventId)
                 .WithName("GetEventAddresses")
                 .WithSummary("Get event addresses")
                 .WithDescription("Retrieves all addresses attached to a specific event.")
                 .Produces<IReadOnlyCollection<EventAddressDto>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPost("/{addressId:guid}", Attach)
                 .WithName("AttachEventAddress")
                 .WithSummary("Attach address to event")
                 .WithDescription("Attaches an existing address to a specific event.")
                 .Produces<EventAddressDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/{addressId:guid}", Detach)
                 .WithName("DetachEventAddress")
                 .WithSummary("Detach address from event")
                 .WithDescription("Detaches an address from a specific event.")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound);
        }

        private static async Task<IResult> GetAllByEventId(Guid eventId, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            var result = await eventAddressService.GetAllByEventIdAsync(eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Attach(Guid eventId, Guid addressId, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            var created = await eventAddressService.AttachAsync(eventId, addressId, cancellationToken);
            return Results.Created($"/api/v1/events/{eventId}/addresses/{addressId}", created);
        }

        private static async Task<IResult> Detach(Guid eventId, Guid addressId, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            await eventAddressService.DetachAsync(eventId, addressId, cancellationToken);
            return Results.NoContent();
        }
    }
}
