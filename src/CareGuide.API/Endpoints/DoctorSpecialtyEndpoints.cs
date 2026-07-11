using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.DoctorSpecialty;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints;

public class DoctorSpecialtyEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/doctors/{doctorId:guid}/specialties")
            .WithTags("Doctor Specialties")
            .WithDefaultProblemResponses();

        group.MapGet("/", GetAll)
             .WithName("GetAllDoctorSpecialties")
             .WithSummary("Get all doctor specialties")
             .WithDescription("Retrieves all specialty records associated with a specific doctor using pagination parameters.")
             .Produces<IReadOnlyCollection<DoctorSpecialtyDto>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/{id:guid}", GetById)
             .WithName("GetDoctorSpecialtyById")
             .WithSummary("Get doctor specialty by id")
             .WithDescription("Retrieves a specific specialty record associated with a specific doctor by identifier.")
             .Produces<DoctorSpecialtyDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
             .WithName("CreateDoctorSpecialty")
             .WithSummary("Create doctor specialty")
             .WithDescription("Creates a new specialty record associated with a specific doctor.")
             .Accepts<CreateDoctorSpecialtyDto>("application/json")
             .Produces<DoctorSpecialtyDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", Update)
             .WithName("UpdateDoctorSpecialty")
             .WithSummary("Update doctor specialty")
             .WithDescription("Updates an existing specialty record associated with a specific doctor by identifier.")
             .Accepts<UpdateDoctorSpecialtyDto>("application/json")
             .Produces<DoctorSpecialtyDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/doctor", DeleteAll)
             .WithName("DeleteAllDoctorSpecialties")
             .WithSummary("Delete all doctor specialties")
             .WithDescription("Deletes all specialty records associated with a specific doctor.")
             .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/", DeleteByIds)
             .WithName("DeleteDoctorSpecialtiesByIds")
             .WithSummary("Delete multiple doctor specialties")
             .WithDescription("Deletes multiple specialty records associated with a specific doctor using a list of identifiers provided in the request body.")
             .Accepts<List<Guid>>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(Guid doctorId, int page, int pageSize, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        page = page == 0 ? PaginationConstants.DefaultPage : page;
        pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

        var result = await doctorSpecialtyService.GetAllByDoctorAsync(page, pageSize, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid doctorId, Guid id, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        var result = await doctorSpecialtyService.GetAsync(id, doctorId, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(Guid doctorId, [FromBody] CreateDoctorSpecialtyDto createDoctorSpecialtyDto, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        var created = await doctorSpecialtyService.CreateAsync(doctorId, createDoctorSpecialtyDto, cancellationToken);
        return Results.Created($"/api/v1/doctors/{doctorId}/specialties/{created.Id}", created);
    }

    private static async Task<IResult> Update(Guid doctorId, Guid id, [FromBody] UpdateDoctorSpecialtyDto updateDoctorSpecialtyDto, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        var result = await doctorSpecialtyService.UpdateAsync(id, doctorId, updateDoctorSpecialtyDto, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAll(Guid doctorId, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        await doctorSpecialtyService.DeleteAllByDoctorAsync(doctorId, cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIds(Guid doctorId, [FromBody] List<Guid> ids, [FromServices] IDoctorSpecialtyService doctorSpecialtyService, CancellationToken cancellationToken)
    {
        await doctorSpecialtyService.DeleteByIdsAsync(ids, doctorId, cancellationToken);
        return Results.NoContent();
    }
}