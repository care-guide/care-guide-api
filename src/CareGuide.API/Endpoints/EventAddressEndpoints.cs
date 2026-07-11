using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.EventAddress;
using CareGuide.Models.DTOs.Shared;

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
                 .WithDescription("Retrieves all addresses attached to a specific event using pagination parameters.")
                 .Produces<PagedResult<EventAddressDto>>(StatusCodes.Status200OK);

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

        private static async Task<IResult> GetAllByEventId(Guid eventId, int page, int pageSize, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await eventAddressService.GetAllByEventIdAsync(page, pageSize, eventId, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Attach(Guid eventId, Guid addressId, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            var created = await eventAddressService.AttachAsync(eventId, addressId, cancellationToken);
            return Results.Created($"/events/{eventId}/addresses/{addressId}", created);
        }

        private static async Task<IResult> Detach(Guid eventId, Guid addressId, IEventAddressService eventAddressService, CancellationToken cancellationToken)
        {
            await eventAddressService.DetachAsync(eventId, addressId, cancellationToken);
            return Results.NoContent();
        }
    }
}
