using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonHealth;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints;

public class PersonHealthEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/person-healths")
            .WithTags("Person Healths")
            .WithDefaultProblemResponses();

        group.MapGet("/", GetAll)
             .WithName("GetAllPersonHealths")
             .WithSummary("Get person health records")
             .WithDescription("Retrieves all health records associated with the authenticated person using pagination parameters.")
             .Produces<List<PersonHealthDto>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetById)
             .WithName("GetPersonHealthById")
             .WithSummary("Get person health by id")
             .WithDescription("Retrieves a specific health record for the authenticated person by its identifier.")
             .Produces<PersonHealthDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
             .WithName("CreatePersonHealth")
             .WithSummary("Create person health")
             .WithDescription("Creates a new health record for the authenticated person.")
             .Accepts<CreatePersonHealthDto>("application/json")
             .Produces<PersonHealthDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", Update)
             .WithName("UpdatePersonHealth")
             .WithSummary("Update person health")
             .WithDescription("Updates an existing health record for the authenticated person by its identifier.")
             .Accepts<UpdatePersonHealthDto>("application/json")
             .Produces<PersonHealthDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/person", DeleteAll)
             .WithName("DeleteAllPersonHealths")
             .WithSummary("Delete person health records")
             .WithDescription("Deletes all health records associated with the authenticated person.")
             .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/", DeleteByIds)
             .WithName("DeletePersonHealthsByIds")
             .WithSummary("Delete multiple person health records")
             .WithDescription("Deletes multiple health records for the authenticated person using a list of identifiers provided in the request body.")
             .Accepts<List<Guid>>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(int page, int pageSize, IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        page = page == 0 ? PaginationConstants.DefaultPage : page;
        pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

        var result = await personHealthService.GetAllByPersonAsync(page, pageSize, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid id, IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        var result = await personHealthService.GetAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(CreatePersonHealthDto createPersonHealth, IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        var created = await personHealthService.CreateAsync(createPersonHealth, cancellationToken);
        return Results.Created($"/api/v1/person-healths/{created.Id}", created);
    }

    private static async Task<IResult> Update(Guid id, UpdatePersonHealthDto updatePersonHealth, IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        var result = await personHealthService.UpdateAsync(id, updatePersonHealth, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAll(IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        await personHealthService.DeleteAllByPersonAsync(cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IPersonHealthService personHealthService, CancellationToken cancellationToken)
    {
        await personHealthService.DeleteByIdsAsync(ids, cancellationToken);
        return Results.NoContent();
    }
}