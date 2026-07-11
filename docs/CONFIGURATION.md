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

The development seed runner also reads the root `.env`. Because it runs on the host rather than inside
the Docker network, it connects to `localhost` using `DB_EXTERNAL_PORT`, plus `DB_NAME`, `DB_USER` and
`DB_PASSWORD`. See [database/seeds/README.md](../database/seeds/README.md).

## Configuration keys

| Key | Description | Example |
| --- | ----------- | ------- |
| `ConnectionStrings:DatabaseConnection` | PostgreSQL connection | `Host=localhost;Port=5434;Database=careguide;Username=...;Password=...` |
| `SecuritySettings:SecretKey` | JWT signing key (**secret**) | *(via env/vault)* |
| `SecuritySettings:Issuer` | Token issuer | `CareGuideAPI` |
| `SecuritySettings:Audience` | Token audience | `CareGuideClient` |
| `CorsOrigins` | Allowed origins (array) | `["https://careguide.dev"]` |
| `AllowedHosts` | Allowed hosts | `*` |
| `Storage:R2:Endpoint` | HTTPS endpoint of the S3-compatible R2 account | `https://<account-id>.r2.cloudflarestorage.com` |
| `Storage:R2:AccountId` | Cloudflare account identifier | *(via env/vault)* |
| `Storage:R2:Bucket` | Private bucket name | `care-guide-private` |
| `Storage:R2:AccessKey` | R2 S3 access key (**secret**) | *(via env/vault)* |
| `Storage:R2:SecretKey` | R2 S3 secret key (**secret**) | *(via env/vault)* |
| `Storage:R2:MinPresignedUrlExpirationSeconds` | Minimum allowed pre-signed URL lifetime | `60` |
| `Storage:R2:MaxPresignedUrlExpirationSeconds` | Maximum allowed pre-signed URL lifetime | `3600` |
| `FileUploads:MaxSizeBytes` | Maximum accepted direct-upload size | `10485760` |
| `FileUploads:AllowedContentTypes` | MIME type allowlist for direct uploads | `["application/pdf", "image/jpeg", "image/png"]` |
| `FileUploads:AllowedExtensions` | Filename extension allowlist for direct uploads | `[".pdf", ".jpg", ".jpeg", ".png"]` |
| `FileUploads:HashAlgorithm` | Required expected-hash algorithm | `SHA256` |
| `FileUploads:HashFormat` | Required expected-hash encoding | `Hex` |
| `FileUploads:PresignedUrlExpirationSeconds` | Direct-upload URL lifetime | `300` |
| `FileDownloads:PresignedUrlExpirationSeconds` | Private download URL lifetime (60–3600 seconds) | `300` |
| `FileLifecycle:PendingUploadExpirationMinutes` | Time before an unattached pending upload is abandoned | `60` |
| `FileLifecycle:UnreferencedFileRetentionDays` | Retention before a completed unreferenced file is purged | `30` |
| `FileLifecycle:CleanupIntervalMinutes` | Interval between idempotent cleanup runs | `15` |
| `Logging:LogLevel` | Log levels | — |

All duration values are integers in seconds, minutes or days as stated in the key name. Byte limits are
integer bytes. Content types and extensions are arrays; extensions include the leading dot. Startup
validation rejects missing R2 settings, non-HTTPS endpoints, empty allowlists, unsupported hash format,
unsafe URL lifetimes and invalid lifecycle periods.

### R2 values and secret delivery

| Cloudflare value | Care Guide key | Secret? |
| ---------------- | -------------- | ------- |
| S3 endpoint | `Storage:R2:Endpoint` | No |
| Account ID | `Storage:R2:AccountId` | Treat as configuration, not a credential |
| Private bucket name | `Storage:R2:Bucket` | Internal configuration |
| Access Key ID | `Storage:R2:AccessKey` | **Yes** |
| Secret Access Key | `Storage:R2:SecretKey` | **Yes** |
| API Token Value | Not consumed by the S3 client | **Yes; do not configure it here** |

Docker Compose reads the `STORAGE_R2_*` placeholders from the gitignored root `.env` and maps them to
.NET's `Storage__R2__*` environment variables. Direct `dotnet run` does not load `.env`; use User Secrets.
Production must inject the same keys from its secret manager. See [FILE_STORAGE.md](FILE_STORAGE.md).

## Ports

| Service | Port | Context |
| ------- | ---- | ------- |
| API (Docker) | `8080` | `docker-compose` |
| PostgreSQL (Docker) | `5434` | `docker-compose` |
| PostgreSQL test database (Docker) | `5435` | `docker-compose` |
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
Storage__R2__Endpoint=https://<account-id>.r2.cloudflarestorage.com
Storage__R2__AccountId=...
Storage__R2__Bucket=...
Storage__R2__AccessKey=...
Storage__R2__SecretKey=...
FileUploads__MaxSizeBytes=10485760
FileUploads__PresignedUrlExpirationSeconds=300
FileDownloads__PresignedUrlExpirationSeconds=300
FileLifecycle__PendingUploadExpirationMinutes=60
FileLifecycle__UnreferencedFileRetentionDays=30
FileLifecycle__CleanupIntervalMinutes=15
```

> In the repository, `SecretKey` and the connection string are empty. Fill them only locally
> (User Secrets) or via environment variable/vault in production — never commit the real values.

---

[Documentation index](README.md) · [Project README](../README.md)
