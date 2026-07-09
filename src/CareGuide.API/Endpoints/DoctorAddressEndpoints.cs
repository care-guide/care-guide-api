using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.DoctorAddress;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints;

public class DoctorAddressEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/doctors/{doctorId:guid}/addresses")
            .WithTags("Doctor Addresses")
            .WithDefaultProblemResponses();

        group.MapGet("/", GetAll)
             .WithName("GetAllDoctorAddresses")
             .WithSummary("Get all doctor addresses")
             .WithDescription("Retrieves all address records attached to a specific doctor using pagination parameters.")
             .Produces<IReadOnlyCollection<DoctorAddressDto>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{addressId:guid}", GetById)
             .WithName("GetDoctorAddressById")
             .WithSummary("Get doctor address by address id")
             .WithDescription("Retrieves a specific address record attached to a specific doctor by address identifier.")
             .Produces<DoctorAddressDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
             .WithName("CreateDoctorAddress")
             .WithSummary("Attach address to doctor")
             .WithDescription("Attaches an existing address to a specific doctor.")
             .Accepts<CreateDoctorAddressDto>("application/json")
             .Produces<DoctorAddressDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/", DeleteByIds)
             .WithName("DeleteDoctorAddressesByIds")
             .WithSummary("Detach multiple addresses from doctor")
             .WithDescription("Detaches multiple address records from a specific doctor using a list of address identifiers provided in the request body.")
             .Accepts<List<Guid>>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/all", DeleteAll)
             .WithName("DeleteAllDoctorAddresses")
             .WithSummary("Detach all addresses from doctor")
             .WithDescription("Detaches all address records from a specific doctor.")
             .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> GetAll(Guid doctorId, int page, int pageSize, IDoctorAddressService doctorAddressService, CancellationToken cancellationToken)
    {
        page = page == 0 ? PaginationConstants.DefaultPage : page;
        pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

        var result = await doctorAddressService.GetAllByDoctorAsync(page, pageSize, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid doctorId, Guid addressId, IDoctorAddressService doctorAddressService, CancellationToken cancellationToken)
    {
        var result = await doctorAddressService.GetAsync(addressId, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(Guid doctorId, CreateDoctorAddressDto createDoctorAddressDto, IDoctorAddressService doctorAddressService, CancellationToken cancellationToken)
    {
        var created = await doctorAddressService.CreateAsync(doctorId, createDoctorAddressDto, cancellationToken);
        return Results.Created($"/doctors/{doctorId}/addresses/{created.AddressId}", created);
    }

    private static async Task<IResult> DeleteAll(Guid doctorId, IDoctorAddressService doctorAddressService, CancellationToken cancellationToken)
    {
        await doctorAddressService.DeleteAllByDoctorAsync(doctorId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIds(Guid doctorId, [FromBody] List<Guid> ids, IDoctorAddressService doctorAddressService, CancellationToken cancellationToken)
    {
        await doctorAddressService.DeleteByIdsAsync(ids, doctorId, cancellationToken);
        return Results.NoContent();
    }
}
