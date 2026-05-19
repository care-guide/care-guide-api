# AGENTS.md Care Guide API

Context guide for AI agents. Read this file before any analysis or changes.

---

## Stack

| Layer          | Technology                             |
| -------------- | -------------------------------------- |
| Runtime        | .NET 10                                |
| API            | Minimal API (ASP.NET Core)             |
| ORM            | Entity Framework Core 10               |
| Database       | PostgreSQL (via Npgsql)                |
| Validation     | FluentValidation                       |
| Mapping        | AutoMapper 16.x                        |
| Authentication | JWT + Refresh Token in cookie          |
| Tests          | xUnit + NSubstitute + FluentAssertions |
| Container      | Docker + docker-compose                |

---

## Architecture

```
CareGuide.API       → Endpoints (Minimal API), Middlewares, Filters
CareGuide.Core      → Service interfaces + implementations (business rules)
CareGuide.Infra     → Repository interfaces + implementations (EF Core, DbContext)
CareGuide.Models    → Entities, DTOs, Enums, Mappers, Validators, Constants
CareGuide.Security  → JWT, PasswordManager (BCrypt + zxcvbn)
CareGuide.Tests     → Unit tests (no DB, no network)
```

Dependency flow: `API → Core → Infra → Models` / `Security ← Core ← API`

---

## Mandatory conventions

### Never inline

Any new type (DTO, entity, enum, mapper, validator, constant) **must be created in its own file** in the correct folder. Never declare types inside other files.

### Where to create each artifact

| Artifact                  | Project            | Path                                                  |
| ------------------------- | ------------------ | ----------------------------------------------------- |
| Entity                    | `CareGuide.Models` | `Entities/<EntityName>.cs`                            |
| Read DTO                  | `CareGuide.Models` | `DTOs/<Domain>/<Domain>Dto.cs`                        |
| Create DTO                | `CareGuide.Models` | `DTOs/<Domain>/Create<Domain>Dto.cs`                  |
| Update DTO                | `CareGuide.Models` | `DTOs/<Domain>/Update<Domain>Dto.cs`                  |
| Enum                      | `CareGuide.Models` | `Enums/<EnumName>.cs`                                 |
| AutoMapper profile        | `CareGuide.Models` | `Mappers/<Domain>/<Domain>ProfileMapper.cs`           |
| Validator                 | `CareGuide.Models` | `Validators/<Domain>/<Action><Domain>DtoValidator.cs` |
| Service interface         | `CareGuide.Core`   | `Interfaces/I<Domain>Service.cs`                      |
| Service implementation    | `CareGuide.Core`   | `Services/<Domain>Service.cs`                         |
| Repository interface      | `CareGuide.Infra`  | `Interfaces/I<Domain>Repository.cs`                   |
| Repository implementation | `CareGuide.Infra`  | `Repositories/<Domain>Repository.cs`                  |
| Endpoints                 | `CareGuide.API`    | `Endpoints/<Domain>Endpoints.cs`                      |
| Service tests             | `CareGuide.Tests`  | `Services/<Domain>ServiceTests.cs`                    |
| Validator tests           | `CareGuide.Tests`  | `Validators/<Action><Domain>DtoValidatorTests.cs`     |
| Mapper tests              | `CareGuide.Tests`  | `Mappers/<Domain>MapperTests.cs`                      |

---

## Code patterns

### Entities

- Inherit from `Entity` (base with `Id`, `CreatedAt`, `UpdatedAt`, `IsActive`)
- Properties marked `required` when mandatory in the database
- Relationships via navigation properties

```csharp
// src/CareGuide.Models/Entities/Shared/Entity.cs
public class Entity : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}
```

### DTOs

- Always `record` with constructor parameters
- Enums annotated with `[JsonConverter(typeof(JsonStringEnumConverter))]`
- **Never return an entity directly**: services and endpoints always return a DTO mapped via AutoMapper
- Entities must never appear in public service method signatures or endpoint responses

```csharp
public record CreateDoctorDto(string Name, string? Details);

// CORRECT
Task<DoctorDto> CreateAsync(CreateDoctorDto dto, CancellationToken ct);

// WRONG never expose entity
Task<Doctor> CreateAsync(CreateDoctorDto dto, CancellationToken ct);
```

### Validators

- Inherit from `AbstractValidator<TDto>`
- Use `DatabaseConstants.MaxLengthStandardText` (255) and `MaxLengthLargeText` (1000)
- Optional fields validated with `.When(x => !string.IsNullOrWhiteSpace(x.Field))`
- Every input DTO (Create, Update, Login, any form) must have a corresponding validator
- Validation is executed automatically by `ValidationFilterFactory`, do not call the validator manually in the endpoint
- Endpoints without an input DTO (e.g. GET by id, DELETE) do not need a validator
- When creating a new input DTO, create the validator in the same commit/PR

### Mappers (AutoMapper)

- Inherit from `Profile`
- Registered via `ServiceCollection.AddAutoMapper()` with `AddLogging()` called first
- Never use standalone `MapperConfiguration` (removed in AutoMapper 16.x)

### Repositories

- Standard repositories implement `IRepository<TEntity>`
- Repositories for entities tied to the logged-in person implement `IBasePersonOwnedRepository<TEntity>`
- Specialized repositories (join tables) have their own interface in `Infra/Interfaces/`

### Services

- Depend only on interfaces (`IRepository`, `IService`)
- Never access `DbContext` directly
- Transactions via `IEfTransactionUnitOfWork` only when **multiple entities are manipulated in a single flow** — ensures atomicity so a mid-flow failure rolls back all changes and leaves the database consistent. Single-entity operations do not need it.
- Throw typed exceptions: `ArgumentException`, `ArgumentNullException`, `KeyNotFoundException`, `UnauthorizedAccessException`, `InvalidOperationException`

### Endpoints (Minimal API)

- Implement `IEndpoint` → method `RegisterEndpoints(IEndpointRouteBuilder)`
- Registered automatically via `EndpointRegistrationExtension` (reflection scan)
- Each endpoint is a separate class in `API/Endpoints/`
- Automatic validation via `ValidationFilterFactory` (do not call validator manually in the endpoint)
- Handlers as `private static async Task<IResult>` methods

```csharp
public class ExampleEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/route").WithTags("Tag").WithDefaultProblemResponses();
        group.MapGet("/", GetAll).Produces<List<ExampleDto>>(200);
        group.MapPost("/", Create).Accepts<CreateExampleDto>("application/json").Produces<ExampleDto>(201);
    }

    private static async Task<IResult> GetAll(IExampleService svc, CancellationToken ct)
        => Results.Ok(await svc.GetAllAsync(ct));

    private static async Task<IResult> Create(CreateExampleDto dto, IExampleService svc, CancellationToken ct)
    {
        var result = await svc.CreateAsync(dto, ct);
        return Results.Created($"/route/{result.Id}", result);
    }
}
```

---

## Tests

> ⚠️ **Mandatory:** After any code change, run the tests before completing the task. No change is considered done without passing tests.

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj
```

Expected result: `Failed: 0`. If any test breaks, the change must be fixed before proceeding.

- All unit tests, no database, no network
- AAA pattern (Arrange / Act / Assert)
- Mocks via NSubstitute (`Substitute.For<IInterface>()`)
- Assertions via FluentAssertions
- To test mappers: use `ServiceCollection` + `AddLogging()` + `AddAutoMapper()`
- Async exceptions: `await act.Should().ThrowAsync<TException>()`
- Rollback in services with UoW: simulate exception with `Task.FromException<T>()`

Full documentation: [`docs/TESTS.md`](docs/TESTS.md)

---

## Relevant enums

```csharp
Gender        { M, F, O }
PhoneType     { R, COM, CEL, O }
BloodType     { A_Positive, A_Negative, B_Positive, B_Negative,
                AB_Positive, AB_Negative, O_Positive, O_Negative,
                RH_Positive, RH_Negative }
DiseaseType   { Oncological, Infectious, Chronic, ... }
```

---

## Useful constants

```csharp
DatabaseConstants.MaxLengthStandardText  // 255
DatabaseConstants.MaxLengthLargeText     // 1000
PaginationConstants.DefaultPage          // default page
PaginationConstants.DefaultPageSize      // default page size
```

---

## Checklist for a new feature (full domain)

Before considering a feature complete, verify:

- [ ] Entity created in `Models/Entities/`
- [ ] DTOs created in `Models/DTOs/<Domain>/` (read, create, update)
- [ ] New enums in `Models/Enums/`
- [ ] AutoMapper profile in `Models/Mappers/<Domain>/`: every output DTO mapped via profile
- [ ] Validators in `Models/Validators/<Domain>/`: required for each input DTO (Create/Update/Login/etc.)
- [ ] No endpoint or service returns an entity directly, only DTO
- [ ] Service interface in `Core/Interfaces/`
- [ ] Service implementation in `Core/Services/`
- [ ] Repository interface in `Infra/Interfaces/`
- [ ] Repository implementation in `Infra/Repositories/`
- [ ] EF Core migration generated and applied
- [ ] Endpoints in `API/Endpoints/`
- [ ] Unit tests for service, validator, and mapper in `Tests/`
- [ ] `dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj` run with `Failed: 0`

---

## Key files for quick reference

| Goal                         | File                                                              |
| ---------------------------- | ----------------------------------------------------------------- |
| Service registration         | `CareGuide.Core/DependencyInjection.cs`                           |
| Model/mapper registration    | `CareGuide.Models/DependencyInjection.cs`                         |
| Entry point + middlewares    | `CareGuide.API/Program.cs`                                        |
| Automatic validation filter  | `CareGuide.API/Filters/ValidationFilterFactory.cs`                |
| Base entity                  | `CareGuide.Models/Entities/Shared/Entity.cs`                      |
| Base repository interface    | `CareGuide.Infra/Interfaces/Shared/IRepository.cs`                |
| Base person-owned repository | `CareGuide.Infra/Interfaces/Shared/IBasePersonOwnedRepository.cs` |
| Full endpoint example        | `CareGuide.API/Endpoints/DoctorEndpoints.cs`                      |
| Full service example         | `CareGuide.Core/Services/DoctorService.cs`                        |
| CI pipeline                  | `.github/workflows/ci-dotnet.yml`                                 |
