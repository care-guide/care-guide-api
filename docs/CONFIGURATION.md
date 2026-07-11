# Configuration

Application settings, ports and secret management.

See also: [EXECUTION.md](EXECUTION.md) · [SECURITY.md](SECURITY.md)

---

## Configuration files

| File | Use |
| ---- | --- |
| `src/CareGuide.API/appsettings.json` | Base configuration (all environments) |
| `src/CareGuide.API/appsettings.Development.json` | Overrides for local development |
| Environment variables | Override the files; **use for secrets** |
| `.env` (project root) | Read by `docker-compose` to substitute `${VAR}` (DB user/password/ports). Gitignored; copy from `.env.example`. |

## Configuration keys

| Key | Description | Example |
| --- | ----------- | ------- |
| `ConnectionStrings:DatabaseConnection` | PostgreSQL connection | `Host=localhost;Port=5434;Database=careguide;Username=...;Password=...` |
| `SecuritySettings:SecretKey` | JWT signing key (**secret**) | *(via env/vault)* |
| `SecuritySettings:Issuer` | Token issuer | `CareGuideAPI` |
| `SecuritySettings:Audience` | Token audience | `CareGuideClient` |
| `CorsOrigins` | Allowed origins (array) | `["https://careguide.dev"]` |
| `AllowedHosts` | Allowed hosts | `*` |
| `Logging:LogLevel` | Log levels | — |

## Ports

| Service | Port | Context |
| ------- | ---- | ------- |
| API (Docker) | `8080` | `docker-compose` |
| PostgreSQL (Docker) | `5434` | `docker-compose` |
| API (IIS Express) | `5000` | local via Visual Studio (see `launchSettings.json`) |

Interactive documentation (Scalar, Development only): `http://localhost:<port>/scalar`.

---

## Secrets

**Never** commit secrets (`SecretKey`, database passwords, connection strings with credentials).

Correct ways to provide secrets:

- **Development:** .NET [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
  (`dotnet user-secrets set "SecuritySettings:SecretKey" "<value>"`).
- **Production/CI:** environment variables or the provider's secrets vault.

Environment variable format (`:` becomes `__`):

```
ConnectionStrings__DatabaseConnection=...
SecuritySettings__SecretKey=...
```

> In the repository, `SecretKey` and the connection string are empty. Fill them only locally
> (User Secrets) or via environment variable/vault in production — never commit the real values.

---

[Documentation index](README.md) · [Project README](../README.md)
