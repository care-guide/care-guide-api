using CareGuide.API.Endpoints.Shared;
using CareGuide.API.Extensions;
using CareGuide.Core.Interfaces;
using CareGuide.Models.Constants;
using CareGuide.Models.DTOs.PersonAnnotation;
using CareGuide.Models.DTOs.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CareGuide.API.Endpoints;

public class PersonAnnotationEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/person-annotations")
            .WithTags("Person Annotations")
            .WithDefaultProblemResponses();

        group.MapGet("/", GetAll)
             .WithName("GetAllPersonAnnotations")
             .WithSummary("Get all annotations")
             .WithDescription("Retrieves all annotations for the authenticated person using pagination parameters.")
             .Produces<PagedResult<PersonAnnotationDto>>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetById)
             .WithName("GetPersonAnnotationById")
             .WithSummary("Get annotation by id")
             .WithDescription("Retrieves a specific annotation for the authenticated person by its identifier.")
             .Produces<PersonAnnotationDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", Create)
             .WithName("CreatePersonAnnotation")
             .WithSummary("Create annotation")
             .WithDescription("Creates a new annotation for the authenticated person.")
             .Accepts<CreatePersonAnnotationDto>("application/json")
             .Produces<PersonAnnotationDto>(StatusCodes.Status201Created)
             .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("/{id:guid}", Update)
             .WithName("UpdatePersonAnnotation")
             .WithSummary("Update annotation")
             .WithDescription("Updates an existing annotation for the authenticated person by its identifier.")
             .Accepts<UpdatePersonAnnotationDto>("application/json")
             .Produces<PersonAnnotationDto>(StatusCodes.Status200OK)
             .ProducesProblem(StatusCodes.Status400BadRequest)
             .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapDelete("/person", DeleteAll)
             .WithName("DeleteAllPersonAnnotations")
             .WithSummary("Delete all annotations")
             .WithDescription("Deletes all annotations associated with the authenticated person.")
             .Produces(StatusCodes.Status204NoContent);

        group.MapDelete("/", DeleteByIds)
             .WithName("DeletePersonAnnotationsByIds")
             .WithSummary("Delete multiple annotations")
             .WithDescription("Deletes multiple annotations for the authenticated person using a list of annotation identifiers provided in the request body.")
             .Accepts<List<Guid>>("application/json")
             .Produces(StatusCodes.Status204NoContent)
             .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    private static async Task<IResult> GetAll(int page, int pageSize, IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        page = page == 0 ? PaginationConstants.DefaultPage : page;
        pageSize = pageSize == 0 ? PaginationConstants.DefaultPageSize : pageSize;

        var result = await personAnnotationService.GetAllByPersonAsync(page, pageSize, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> GetById(Guid id, IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        var result = await personAnnotationService.GetAsync(id, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> Create(CreatePersonAnnotationDto createPersonAnnotation, IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        var created = await personAnnotationService.CreateAsync(createPersonAnnotation, cancellationToken);
        return Results.Created($"/person-annotations/{created.Id}", created);
    }

    private static async Task<IResult> Update(Guid id, UpdatePersonAnnotationDto updatePersonAnnotation, IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        var result = await personAnnotationService.UpdateAsync(id, updatePersonAnnotation, cancellationToken);
        return Results.Ok(result);
    }

    private static async Task<IResult> DeleteAll(IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        await personAnnotationService.DeleteAllByPersonAsync(cancellationToken);
        return Results.NoContent();
    }

    private static async Task<IResult> DeleteByIds([FromBody] List<Guid> ids, IPersonAnnotationService personAnnotationService, CancellationToken cancellationToken)
    {
        await personAnnotationService.DeleteByIdsAsync(ids, cancellationToken);
        return Results.NoContent();
    }
}