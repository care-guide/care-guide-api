using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.DoctorPhone;
using CareGuide.Models.DTOs.Phone;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints;

public class DoctorPhoneEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/doctors/{doctorId:guid}/phones")
            .WithTags("Doctor Phones")
            .WithDefaultProblemResponses();

        group.MapGet("/", GetAll)
             .WithName("GetAllDoctorPhones")
             .WithSummary("Get all doctor phones")
             .WithDescription("Retrieves all phone records associated with a specific doctor using pagination parameters.")
             .Produces<IReadOnlyCollection<DoctorPhoneDto>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{phoneId:guid}", GetById)
             .WithName("GetDoctorPhoneById")
             .WithSummary("Get doctor phone by phone id")
             .WithDescription("Retrieves a specific phone record associated with a specific doctor by phone identifier.")
             .Produces<DoctorPhoneDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
             .WithName("CreateDoctorPhone")
             .WithSummary("Create doctor phone")
             .WithDescription("Creates a new phone record associated with a specific doctor.")
             .Accepts<CreatePhoneDto>("application/json")
             .Produces<DoctorPhoneDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{phoneId:guid}", Update)
             .WithName("UpdateDoctorPhone")
             .WithSummary("Update doctor phone")
             .WithDescription("Updates an existing phone record associated with a specific doctor by phone identifier.")
             .Accepts<UpdatePhoneDto>("application/json")
             .Produces<DoctorPhoneDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/", DeleteByIds)
             .WithName("DeleteDoctorPhonesByIds")
             .WithSummary("Delete multiple doctor phones")
             .WithDescription("Deletes multiple phone records associated with a specific doctor using a list of phone identifiers provided in the request body.")
             .Accepts<List<Guid>>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapDelete("/doctor", DeleteAll)
             .WithName("DeleteAllDoctorPhones")
             .WithSummary("Delete all doctor phones")
             .WithDescription("Deletes all phone records associated with a specific doctor.")
             .Produces(StatusCodes.Status204NoContent);
    }

    private static async Task<IResult> GetAll(Guid doctorId, int page, int pageSize, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        page = page == 0 ? PaginationConstants.DefaultPage : page;
        pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

        var result = await doctorPhoneService.GetAllByDoctorAsync(page, pageSize, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid doctorId, Guid phoneId, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        var result = await doctorPhoneService.GetAsync(phoneId, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(Guid doctorId, CreatePhoneDto createPhoneDto, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        var created = await doctorPhoneService.CreateAsync(doctorId, createPhoneDto, cancellationToken);
        return Results.Created($"/api/v1/doctors/{doctorId}/phones/{created.PhoneId}", created);
    }

    private static async Task<IResult> Update(Guid doctorId, Guid phoneId, UpdatePhoneDto updatePhoneDto, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        var result = await doctorPhoneService.UpdateAsync(phoneId, doctorId, updatePhoneDto, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAll(Guid doctorId, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        await doctorPhoneService.DeleteAllByDoctorAsync(doctorId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIds(Guid doctorId, [FromBody] List<Guid> ids, IDoctorPhoneService doctorPhoneService, CancellationToken cancellationToken)
    {
        await doctorPhoneService.DeleteByIdsAsync(ids, doctorId, cancellationToken);
        return Results.NoContent();
    }
}