# Migrations Guide (Entity Framework Core)

This document explains how to create and apply migrations in the project using Entity Framework Core with PostgreSQL.

> The commands below assume execution from the `src/` folder. Adjust the `--project` paths according to
> your working directory. The connection string must be configured — see [CONFIGURATION.md](CONFIGURATION.md).

## Prerequisite

Before running the migration commands, make sure the `dotnet-ef` tool is installed:

```sh
 dotnet tool install --global dotnet-ef
```

## Main Commands

### 1. Create a new migration

```sh
 dotnet ef migrations add MigrationName --project CareGuide.Infra --startup-project CareGuide.API
```

- `MigrationName`: Descriptive name for the migration (e.g. `AddPatientTable`).
- The `--project` parameter indicates where the context and migration classes live.
- The `--startup-project` parameter indicates the application startup project.

### 2. Apply a migration to the database

```sh
 dotnet ef database update --project CareGuide.Infra --startup-project CareGuide.API
```

- Applies all pending migrations.

## Official Documentation

For more commands and options, see the official Entity Framework Core documentation:

- [EF Core CLI Reference](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)
- [Migrations Overview](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)

---

[Documentation index](README.md) · [Project README](../README.md)
