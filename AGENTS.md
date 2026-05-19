# AGENTS.md — Care Guide API

Guia de contexto para agentes de IA. Leia este arquivo antes de qualquer análise ou alteração.

---

## Stack

| Camada       | Tecnologia                             |
| ------------ | -------------------------------------- |
| Runtime      | .NET 10                                |
| API          | Minimal API (ASP.NET Core)             |
| ORM          | Entity Framework Core 10               |
| Banco        | PostgreSQL (via Npgsql)                |
| Validação    | FluentValidation                       |
| Mapeamento   | AutoMapper 16.x                        |
| Autenticação | JWT + Refresh Token em cookie          |
| Testes       | xUnit + NSubstitute + FluentAssertions |
| Container    | Docker + docker-compose                |

---

## Arquitetura

```
CareGuide.API       → Endpoints (Minimal API), Middlewares, Filtros
CareGuide.Core      → Interfaces de serviço + implementações (regras de negócio)
CareGuide.Infra     → Interfaces de repositório + implementações (EF Core, DbContext)
CareGuide.Models    → Entities, DTOs, Enums, Mappers, Validators, Constants
CareGuide.Security  → JWT, PasswordManager (BCrypt + zxcvbn)
CareGuide.Tests     → Testes unitários (sem DB, sem rede)
```

Fluxo de dependência: `API → Core → Infra → Models` / `Security ← Core ← API`

---

## Convenções obrigatórias

### Nunca fazer inline

Qualquer tipo novo (DTO, entity, enum, mapper, validator, constante) **deve ser criado em arquivo próprio** na pasta correta. Nunca declarar tipos dentro de outros arquivos.

### Onde criar cada artefato

| Artefato                     | Projeto            | Caminho                                               |
| ---------------------------- | ------------------ | ----------------------------------------------------- |
| Entity                       | `CareGuide.Models` | `Entities/<NomeEntity>.cs`                            |
| DTO de leitura               | `CareGuide.Models` | `DTOs/<Dominio>/<Dominio>Dto.cs`                      |
| DTO de criação               | `CareGuide.Models` | `DTOs/<Dominio>/Create<Dominio>Dto.cs`                |
| DTO de atualização           | `CareGuide.Models` | `DTOs/<Dominio>/Update<Dominio>Dto.cs`                |
| Enum                         | `CareGuide.Models` | `Enums/<NomeEnum>.cs`                                 |
| AutoMapper profile           | `CareGuide.Models` | `Mappers/<Dominio>/<Dominio>ProfileMapper.cs`         |
| Validator                    | `CareGuide.Models` | `Validators/<Dominio>/<Acao><Dominio>DtoValidator.cs` |
| Interface de serviço         | `CareGuide.Core`   | `Interfaces/I<Dominio>Service.cs`                     |
| Implementação de serviço     | `CareGuide.Core`   | `Services/<Dominio>Service.cs`                        |
| Interface de repositório     | `CareGuide.Infra`  | `Interfaces/I<Dominio>Repository.cs`                  |
| Implementação de repositório | `CareGuide.Infra`  | `Repositories/<Dominio>Repository.cs`                 |
| Endpoints                    | `CareGuide.API`    | `Endpoints/<Dominio>Endpoints.cs`                     |
| Testes de serviço            | `CareGuide.Tests`  | `Services/<Dominio>ServiceTests.cs`                   |
| Testes de validator          | `CareGuide.Tests`  | `Validators/<Acao><Dominio>DtoValidatorTests.cs`      |
| Testes de mapper             | `CareGuide.Tests`  | `Mappers/<Dominio>MapperTests.cs`                     |

---

## Padrões de código

### Entities

- Herdam de `Entity` (base com `Id`, `CreatedAt`, `UpdatedAt`, `IsActive`)
- Propriedades com `required` quando obrigatórias no banco
- Relacionamentos via navigation properties

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

- Sempre `record` com parâmetros no construtor
- Enums anotados com `[JsonConverter(typeof(JsonStringEnumConverter))]`
- **Nunca retornar uma entity diretamente** — serviços e endpoints sempre retornam DTO mapeado via AutoMapper
- Entity jamais deve aparecer em assinatura de método público de serviço ou em response de endpoint

```csharp
public record CreateDoctorDto(string Name, string? Details);

// CORRETO
Task<DoctorDto> CreateAsync(CreateDoctorDto dto, CancellationToken ct);

// ERRADO — nunca expor entity
Task<Doctor> CreateAsync(CreateDoctorDto dto, CancellationToken ct);
```

### Validators

- Herdam de `AbstractValidator<TDto>`
- Usar `DatabaseConstants.MaxLengthStandardText` (255) e `MaxLengthLargeText` (1000)
- Campos opcionais validados com `.When(x => !string.IsNullOrWhiteSpace(x.Campo))`
- Todo DTO de entrada (Create, Update, Login, qualquer formulário) deve ter validator correspondente
- A validação é executada automaticamente pelo `ValidationFilterFactory` — não chamar validator manualmente no endpoint
- Endpoints sem DTO de entrada (ex: GET por id, DELETE) não precisam de validator
- Ao criar novo DTO de entrada, criar o validator no mesmo commit/PR

### Mappers (AutoMapper)

- Herdam de `Profile`
- Registrados via `ServiceCollection.AddAutoMapper()` com `AddLogging()` antes
- Nunca usar `MapperConfiguration` standalone (removido no AutoMapper 16.x)

### Repositórios

- Repositórios padrão implementam `IRepository<TEntity>`
- Repositórios de entidades vinculadas à pessoa logada implementam `IBasePersonOwnedRepository<TEntity>`
- Repositórios especializados (join tables) têm interface própria em `Infra/Interfaces/`

### Serviços

- Dependem apenas de interfaces (`IRepository`, `IService`)
- Nunca acessam `DbContext` diretamente
- Transações via `IEfTransactionUnitOfWork` (Begin → Commit → Rollback em catch)
- Lançam exceções tipadas: `ArgumentException`, `ArgumentNullException`, `KeyNotFoundException`, `UnauthorizedAccessException`, `InvalidOperationException`

### Endpoints (Minimal API)

- Implementam `IEndpoint` → método `RegisterEndpoints(IEndpointRouteBuilder)`
- Registrados automaticamente via `EndpointRegistrationExtension` (scan por reflection)
- Cada endpoint é uma classe separada em `API/Endpoints/`
- Validação automática via `ValidationFilterFactory` (não chamar validator manualmente no endpoint)
- Handlers como métodos `private static async Task<IResult>`

```csharp
public class ExemploEndpoints() : IEndpoint
{
    public void RegisterEndpoints(IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/rota").WithTags("Tag").WithDefaultProblemResponses();
        group.MapGet("/", GetAll).Produces<List<ExemploDto>>(200);
        group.MapPost("/", Create).Accepts<CreateExemploDto>("application/json").Produces<ExemploDto>(201);
    }

    private static async Task<IResult> GetAll(IExemploService svc, CancellationToken ct)
        => Results.Ok(await svc.GetAllAsync(ct));

    private static async Task<IResult> Create(CreateExemploDto dto, IExemploService svc, CancellationToken ct)
    {
        var result = await svc.CreateAsync(dto, ct);
        return Results.Created($"/rota/{result.Id}", result);
    }
}
```

---

## Testes

> ⚠️ **Obrigatório:** Após qualquer alteração de código, execute os testes antes de concluir a tarefa. Nenhuma mudança é considerada completa sem os testes passando.

```bash
dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj
```

Resultado esperado: `Com falha: 0`. Se algum teste quebrar, a alteração deve ser corrigida antes de prosseguir.

- Todos unitários, sem banco, sem rede
- Padrão AAA (Arrange / Act / Assert)
- Mocks via NSubstitute (`Substitute.For<IInterface>()`)
- Assertions via FluentAssertions
- Para testar mappers: usar `ServiceCollection` + `AddLogging()` + `AddAutoMapper()`
- Exceções async: `await act.Should().ThrowAsync<TException>()`
- Rollback em serviços com UoW: simular exceção com `Task.FromException<T>()`

Documentação completa: [`docs/TESTS.md`](docs/TESTS.md)

---

## Enums relevantes

```csharp
Gender        { M, F, O }
PhoneType     { R, COM, CEL, O }
BloodType     { A_Positive, A_Negative, B_Positive, B_Negative,
                AB_Positive, AB_Negative, O_Positive, O_Negative,
                RH_Positive, RH_Negative }
DiseaseType   { Oncological, Infectious, Chronic, ... }
```

---

## Constantes úteis

```csharp
DatabaseConstants.MaxLengthStandardText  // 255
DatabaseConstants.MaxLengthLargeText     // 1000
PaginationConstants.DefaultPage          // página padrão
PaginationConstants.DefaultPageSize      // tamanho padrão
```

---

## Checklist para nova feature (domínio completo)

Antes de considerar uma feature completa, verificar:

- [ ] Entity criada em `Models/Entities/`
- [ ] DTOs criados em `Models/DTOs/<Dominio>/` (leitura, criação, atualização)
- [ ] Enums novos em `Models/Enums/`
- [ ] AutoMapper profile em `Models/Mappers/<Dominio>/` — todo DTO de saída mapeado via profile
- [ ] Validators em `Models/Validators/<Dominio>/` — obrigatório para cada DTO de entrada (Create/Update/Login/etc.)
- [ ] Nenhum endpoint ou serviço retorna entity diretamente — somente DTO
- [ ] Interface de serviço em `Core/Interfaces/`
- [ ] Implementação de serviço em `Core/Services/`
- [ ] Interface de repositório em `Infra/Interfaces/`
- [ ] Implementação de repositório em `Infra/Repositories/`
- [ ] Migration do EF Core gerada e aplicada
- [ ] Endpoints em `API/Endpoints/`
- [ ] Testes unitários de serviço, validator e mapper em `Tests/`
- [ ] `dotnet test src/CareGuide.Tests/CareGuide.Tests.csproj` executado com `Com falha: 0`

---

## Arquivos-chave para leitura rápida

| Objetivo                       | Arquivo                                                           |
| ------------------------------ | ----------------------------------------------------------------- |
| Registro de serviços           | `CareGuide.Core/DependencyInjection.cs`                           |
| Registro de modelos/mappers    | `CareGuide.Models/DependencyInjection.cs`                         |
| Entry point + middlewares      | `CareGuide.API/Program.cs`                                        |
| Filtro de validação automática | `CareGuide.API/Filters/ValidationFilterFactory.cs`                |
| Base entity                    | `CareGuide.Models/Entities/Shared/Entity.cs`                      |
| Base repository interface      | `CareGuide.Infra/Interfaces/Shared/IRepository.cs`                |
| Base person-owned repository   | `CareGuide.Infra/Interfaces/Shared/IBasePersonOwnedRepository.cs` |
| Exemplo completo de endpoint   | `CareGuide.API/Endpoints/DoctorEndpoints.cs`                      |
| Exemplo completo de serviço    | `CareGuide.Core/Services/DoctorService.cs`                        |
| CI pipeline                    | `.github/workflows/ci-dotnet.yml`                                 |
