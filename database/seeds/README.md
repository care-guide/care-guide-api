# Development database seed

Populates every application table with deterministic, relationally valid data for a normal user and an administrator. The script is intended only for local development.

## Prerequisites

- Apply EF Core migrations first.
- Start the PostgreSQL service from `docker-compose.yml`.
- Keep the database settings in the repository root `.env`.

## Run

```powershell
dotnet run --project database/seeds/CareGuide.Seeds.csproj -- --env-file .env
```

The seed is idempotent: rerunning it replaces only records owned by its deterministic IDs and preserves unrelated local data.

## Development accounts

| Role | Email | Password |
| --- | --- | --- |
| User | `user@careguide.local` | `CareGuide@User2026!` |
| Admin | `admin@careguide.local` | `CareGuide@Admin2026!` |

Passwords are hashed at execution time with the same `PasswordManagerHelper` used by the API. Do not use these accounts or passwords outside local development.
