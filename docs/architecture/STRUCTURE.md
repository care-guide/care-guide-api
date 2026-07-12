# Solution Structure

Below is a description of the main projects and folders that make up the Care Guide solution.
For the code conventions of each layer (where to create each artifact, DTO/validator/mapper patterns),
see [AGENTS.md](../../AGENTS.md).

```text
src/
    CareGuide.API/      → Endpoints (Minimal API), Middlewares, Filters, Extensions
    CareGuide.Core/     → Service interfaces and implementations (business rules)
    CareGuide.Infra/    → Repositories, DbContext, data access (EF Core)
    CareGuide.Models/   → Entities, DTOs, Enums, Mappers, Validators, Constants
    CareGuide.Security/ → JWT, password hashing and strength
    CareGuide.Tests/    → Unit tests (no database, no network)
database/
    seeds/              → Idempotent local-development database seed and runner
```

`database/seeds` is development tooling rather than an application layer. Its runner reads the local
`.env`, hashes development passwords through `CareGuide.Security`, and populates relationally valid
data after migrations have been applied. See [the seed guide](../../database/seeds/README.md).

## Dependency flow

```
API → Core → Infra → Models
API → Security ;  Core → Security
```

Each layer depends only on the interfaces of the inner layers. Services (`Core`) never access
`DbContext` directly — only through repositories (`Infra`).

### CareGuide.API

Responsible for the endpoints (Minimal API) that receive and respond to HTTP requests, and for managing
the API flow.

### CareGuide.Core

Contains the application's business logic, including rules, validations and core services.

### CareGuide.Infra

Manages database access, data persistence and repository implementations.

### CareGuide.Models

Defines the data models (entities), DTOs (Data Transfer Objects), enums and other models used
throughout the solution.

### CareGuide.Security

Manages security features such as authentication, authorization, hashing and validation of user
passwords.

### CareGuide.Tests

Project responsible for the solution's unit and integration tests. Ensures quality and correct behavior
of the implemented features, following good test automation practices. See [TESTS.md](../development/TESTS.md).

---

[Documentation index](../README.md) · [Project README](../../README.md)
