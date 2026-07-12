# Migrations Guide (Entity Framework Core)

This document explains how to create and apply migrations in the project using Entity Framework Core with PostgreSQL.

> The commands below assume execution from the repository root. The connection string must be
> configured — see [CONFIGURATION.md](../getting-started/CONFIGURATION.md).

## Prerequisite

Before running the migration commands, make sure the `dotnet-ef` tool is installed:

```sh
 dotnet tool install --global dotnet-ef
```

## Main Commands

### 1. Create a new migration

```sh
dotnet ef migrations add MigrationName --project src/CareGuide.Infra --startup-project src/CareGuide.API
```

- `MigrationName`: Descriptive name for the migration (e.g. `AddPatientTable`).
- The `--project` parameter indicates where the context and migration classes live.
- The `--startup-project` parameter indicates the application startup project.

### 2. Apply a migration to the database

```sh
dotnet ef database update --project src/CareGuide.Infra --startup-project src/CareGuide.API
```

- Applies all pending migrations.

### 3. Populate development data (optional)

After applying migrations to the local Docker database, populate all application tables with
relationally valid data for a normal user and an administrator:

```sh
dotnet run --project database/seeds/CareGuide.Seeds.csproj -- --env-file .env
```

The seed is idempotent and intended only for local development. Account credentials and behavior are
documented in [database/seeds/README.md](../../database/seeds/README.md).

## Official Documentation

For more commands and options, see the official Entity Framework Core documentation:

- [EF Core CLI Reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

[Documentation index](../README.md) · [Project README](../../README.md)
