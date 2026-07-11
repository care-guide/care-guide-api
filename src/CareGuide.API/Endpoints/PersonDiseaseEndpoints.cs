using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonDisease;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class PersonDiseaseEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/person-diseases")
                .WithTags("Person Diseases")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAll)
                 .WithName("GetAllPersonDiseases")
                 .WithSummary("Get all diseases")
                 .WithDescription("Retrieves all diseases for the authenticated person using pagination parameters.")
                 .Produces<List<PersonDiseaseDto>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetPersonDiseaseById")
                 .WithSummary("Get disease by id")
                 .WithDescription("Retrieves a specific disease for the authenticated person by its identifier.")
                 .Produces<PersonDiseaseDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreatePersonDisease")
                 .WithSummary("Create disease")
                 .WithDescription("Creates a new disease for the authenticated person.")
                 .Accepts<CreatePersonDiseaseDto>("application/json")
                 .Produces<PersonDiseaseDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:guid}", Update)
                 .WithName("UpdatePersonDisease")
                 .WithSummary("Update disease")
                 .WithDescription("Updates an existing disease for the authenticated person by its identifier.")
                 .Accepts<UpdatePersonDiseaseDto>("application/json")
                 .Produces<PersonDiseaseDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/person", DeleteAll)
                 .WithName("DeleteAllPersonDiseases")
                 .WithSummary("Delete all diseases")
                 .WithDescription("Deletes all diseases associated with the authenticated person.")
                 .Produces(StatusCodes.Status204NoContent);

            group.MapDelete("/", DeleteByIds)
                 .WithName("DeletePersonDiseasesByIds")
                 .WithSummary("Delete multiple diseases")
                 .WithDescription("Deletes multiple diseases for the authenticated person using a list of disease identifiers provided in the request body.")
                 .Accepts<List<Guid>>("application/json")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> GetAll(int page, int pageSize, IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await personDiseaseService.GetAllByPersonAsync(page, pageSize, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(Guid id, IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            var result = await personDiseaseService.GetAsync(id, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(CreatePersonDiseaseDto createPersonDisease, IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            var created = await personDiseaseService.CreateAsync(createPersonDisease, cancellationToken);
            return Results.Created($"/api/v1/person-diseases/{created.Id}", created);
        }

        private static async Task<IResult> Update(Guid id, UpdatePersonDiseaseDto updatePersonDisease, IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            var result = await personDiseaseService.UpdateAsync(id, updatePersonDisease, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteAll(IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            await personDiseaseService.DeleteAllByPersonAsync(cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IPersonDiseaseService personDiseaseService, CancellationToken cancellationToken)
        {
            await personDiseaseService.DeleteByIdsAsync(ids, cancellationToken);
            return Results.NoContent();
        }
    }
}
