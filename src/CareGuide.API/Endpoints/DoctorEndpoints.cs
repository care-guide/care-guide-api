using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.Doctor;
using CareGuide.Models.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class DoctorEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/doctors")
                .WithTags("Doctors")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAll)
                 .WithName("GetAllDoctors")
                 .WithSummary("Get all doctors")
                 .WithDescription("Retrieves all doctors for the authenticated person using pagination parameters.")
                 .Produces<PagedResult<DoctorDto>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetDoctorById")
                 .WithSummary("Get doctor by id")
                 .WithDescription("Retrieves a specific doctor for the authenticated person by its identifier.")
                 .Produces<DoctorDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreateDoctor")
                 .WithSummary("Create doctor")
                 .WithDescription("Creates a new doctor for the authenticated person.")
                 .Accepts<CreateDoctorDto>("application/json")
                 .Produces<DoctorDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:guid}", Update)
                 .WithName("UpdateDoctor")
                 .WithSummary("Update doctor")
                 .WithDescription("Updates an existing doctor for the authenticated person by its identifier.")
                 .Accepts<UpdateDoctorDto>("application/json")
                 .Produces<DoctorDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/person", DeleteAll)
                 .WithName("DeleteAllDoctors")
                 .WithSummary("Delete all doctors")
                 .WithDescription("Deletes all doctors associated with the authenticated person.")
                 .Produces(StatusCodes.Status204NoContent);

            group.MapDelete("/", DeleteByIds)
                 .WithName("DeleteDoctorsByIds")
                 .WithSummary("Delete multiple doctors")
                 .WithDescription("Deletes multiple doctors for the authenticated person using a list of doctor identifiers provided in the request body.")
                 .Accepts<List<Guid>>("application/json")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> GetAll(int page, int pageSize, IDoctorService doctorService, CancellationToken cancellationToken)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await doctorService.GetAllByPersonAsync(page, pageSize, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(Guid id, IDoctorService doctorService, CancellationToken cancellationToken)
        {
            var result = await doctorService.GetAsync(id, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(CreateDoctorDto createDoctorDto, IDoctorService doctorService, CancellationToken cancellationToken)
        {
            var created = await doctorService.CreateAsync(createDoctorDto, cancellationToken);
            return Results.Created($"/doctors/{created.Id}", created);
        }

        private static async Task<IResult> Update(Guid id, UpdateDoctorDto updateDoctorDto, IDoctorService doctorService, CancellationToken cancellationToken)
        {
            var result = await doctorService.UpdateAsync(id, updateDoctorDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteAll(IDoctorService doctorService, CancellationToken cancellationToken)
        {
            await doctorService.DeleteAllByPersonAsync(cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IDoctorService doctorService, CancellationToken cancellationToken)
        {
            await doctorService.DeleteByIdsAsync(ids, cancellationToken);
            return Results.NoContent();
        }
    }
}
