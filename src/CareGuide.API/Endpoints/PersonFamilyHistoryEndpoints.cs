using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonFamilyHistory;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints
{
    public class PersonFamilyHistoryEndpoints() : IEndpoint
    {
        public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
        {
            var group = endpoints
                .MapGroup("/person-family-histories")
                .WithTags("Person Family Histories")
                .WithDefaultProblemResponses();

            group.MapGet("/", GetAll)
                 .WithName("GetAllPersonFamilyHistories")
                 .WithSummary("Get all family histories")
                 .WithDescription("Retrieves all family histories for the authenticated person using pagination parameters.")
                 .Produces<List<PersonFamilyHistoryDto>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapGet("/{id:guid}", GetById)
                 .WithName("GetPersonFamilyHistoryById")
                 .WithSummary("Get family history by id")
                 .WithDescription("Retrieves a specific family history for the authenticated person by its identifier.")
                 .Produces<PersonFamilyHistoryDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapPost("/", Create)
                 .WithName("CreatePersonFamilyHistory")
                 .WithSummary("Create family history")
                 .WithDescription("Creates a new family history for the authenticated person.")
                 .Accepts<CreatePersonFamilyHistoryDto>("application/json")
                 .Produces<PersonFamilyHistoryDto>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest);

            group.MapPut("/{id:guid}", Update)
                 .WithName("UpdatePersonFamilyHistory")
                 .WithSummary("Update family history")
                 .WithDescription("Updates an existing family history for the authenticated person by its identifier.")
                 .Accepts<UpdatePersonFamilyHistoryDto>("application/json")
                 .Produces<PersonFamilyHistoryDto>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status404NotFound);

            group.MapDelete("/person", DeleteAll)
                 .WithName("DeleteAllFamilyHistories")
                 .WithSummary("Delete all family histories")
                 .WithDescription("Deletes all family histories associated with the authenticated person.")
                 .Produces(StatusCodes.Status204NoContent);

            group.MapDelete("/", DeleteByIds)
                 .WithName("DeleteFamilyHistoriesByIds")
                 .WithSummary("Delete multiple family histories")
                 .WithDescription("Deletes multiple family histories for the authenticated person using a list of family history identifiers provided in the request body.")
                 .Accepts<List<Guid>>("application/json")
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status400BadRequest);
        }

        private static async Task<IResult> GetAll(int page, int pageSize, IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            page = page == 0 ? PaginationConstants.DefaultPage : page;
            pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

            var result = await personFamilyHistoryService.GetAllByPersonAsync(page, pageSize, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> GetById(Guid id, IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            var result = await personFamilyHistoryService.GetAsync(id, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> Create(CreatePersonFamilyHistoryDto createPersonFamilyHistoryDto, IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            var created = await personFamilyHistoryService.CreateAsync(createPersonFamilyHistoryDto, cancellationToken);
            return Results.Created($"/api/v1/person-family-histories/{created.Id}", created);
        }

        private static async Task<IResult> Update(Guid id, UpdatePersonFamilyHistoryDto updatePersonFamilyHistoryDto, IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            var result = await personFamilyHistoryService.UpdateAsync(id, updatePersonFamilyHistoryDto, cancellationToken);
            return Results.Ok(result);
        }

        private static async Task<IResult> DeleteAll(IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            await personFamilyHistoryService.DeleteAllByPersonAsync(cancellationToken);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IPersonFamilyHistoryService personFamilyHistoryService, CancellationToken cancellationToken)
        {
            await personFamilyHistoryService.DeleteByIdsAsync(ids, cancellationToken);
            return Results.NoContent();
        }
    }
}
