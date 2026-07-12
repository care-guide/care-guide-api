# Private file storage with Cloudflare R2

Care Guide stores health-related attachments in a **private** Cloudflare R2 bucket. The API authorizes
each operation and returns short-lived S3-compatible pre-signed URLs; credentials, bucket names and
storage keys are never returned to clients.

See also: [CONFIGURATION.md](../getting-started/CONFIGURATION.md) · [API.md](API.md) · [SECURITY.md](../security/SECURITY.md) ·
[R2_RUNBOOK.md](../operations/R2_RUNBOOK.md)

## Development setup

1. In Cloudflare, create a private R2 bucket. Do not enable public access or a public custom domain.
2. Create an R2 API token scoped to that bucket with object read/write permission.
3. Record the S3 `Access Key ID` and `Secret Access Key`; the token value itself is not used by the AWS SDK.
4. Copy the account ID and S3 endpoint (`https://<account-id>.r2.cloudflarestorage.com`).
5. Supply secrets through .NET User Secrets for `dotnet run`, or `.env` plus Docker Compose.

```powershell
dotnet user-secrets set "Storage:R2:Endpoint" "https://<account-id>.r2.cloudflarestorage.com" --project src/CareGuide.API
dotnet user-secrets set "Storage:R2:AccountId" "<account-id>" --project src/CareGuide.API
dotnet user-secrets set "Storage:R2:Bucket" "<private-bucket>" --project src/CareGuide.API
dotnet user-secrets set "Storage:R2:AccessKey" "<access-key-id>" --project src/CareGuide.API
dotnet user-secrets set "Storage:R2:SecretKey" "<secret-access-key>" --project src/CareGuide.API
```

Use a secrets vault or injected environment variables in production. Never put real values in
`appsettings*.json`, `.env.example`, logs, issue descriptions, screenshots or API responses.

## HTTP flow

All routes below use the `/api/v1` prefix and require authentication.

### 1. Initiate

`POST /api/v1/events/{eventId}/files/uploads`

```json
{
  "name": "exam-result.pdf",
  "size": 245760,
  "contentType": "application/pdf",
  "expectedHash": "<64-character-lowercase-or-uppercase-SHA256-hex>"
}
```

The event must belong to the authenticated person. The API validates size, MIME type, extension and
SHA-256 format, creates a pending `FileAsset`, and generates an opaque server-side storage key.

```json
{
  "fileId": "00000000-0000-0000-0000-000000000001",
  "uploadUrl": "https://<r2-host>/<redacted-presigned-request>",
  "method": "PUT",
  "requiredHeaders": {
    "Content-Type": "application/pdf",
    "x-amz-meta-sha256": "<same-SHA256-hex>"
  },
  "expiresAt": "2030-01-01T12:05:00Z"
}
```

### 2. Upload directly to R2

Send the bytes to `uploadUrl` using exactly the returned method and required headers. Do not send the
binary to the Care Guide API. A pre-signed URL authorizes one `PUT` to one opaque object key and expires
after the configured interval.

### 3. Confirm and attach

`POST /api/v1/events/{eventId}/files/{fileId}/confirm`

The API performs an R2 `HEAD`, checks object existence, length, MIME type and signed SHA-256 metadata,
then atomically marks the file uploaded and creates `EventFile`. Repeating a successful confirmation is
idempotent.

R2 `ETag` is not treated as SHA-256. Signed metadata proves that the declared digest was not changed
between initiation and upload, but does not independently hash stored bytes. Strict cryptographic
verification requires a trusted process to stream and hash the object.

### 4. Download

`GET /api/v1/events/{eventId}/files/{fileId}/download`

The attachment, event and file must belong to the authenticated person, the file must be completed,
and the object must still exist. The response contains a short-lived GET-only URL with a sanitized,
signed `Content-Disposition` filename. The API never persists this URL.

### 5. Detach and delete

- `DELETE /api/v1/events/{eventId}/files/{fileId}` removes only the `EventFile` association.
- `DELETE /api/v1/files/{fileId}` permanently deletes only an owned file with zero references.

Definitive deletion records a recoverable state, deletes the R2 object, and then deletes the database
row. Event deletion removes associations but does not cascade-delete shared file objects. Pending
uploads and retained unreferenced files are cleaned by the configured background routine.

## Expected failures

Errors use the standard `application/problem+json` format documented in [API.md](API.md).

| Scenario | Expected status |
| -------- | --------------- |
| Invalid size, extension, MIME, hash, or lifecycle state | `400` |
| Event/file outside the authenticated person's scope | `404` or the existing authorization error mapping |
| Object missing during confirmation or download | `404` |
| File still referenced during definitive deletion | `400` |
| R2 or database operation fails | Non-success; deletion remains retryable/observable |

## Production checklist

- Keep the bucket private and scope credentials to the required bucket.
- Inject credentials from a secret manager and define a rotation owner/schedule.
- Review upload allowlists, size limits, URL lifetimes and retention with product/security owners.
- Configure alerts for cleanup failures, deletion failures, storage growth and operation volume.
- Confirm contractual, residency, retention and deletion requirements with legal/privacy stakeholders.
- Test backup and incident-response procedures without copying production health data into development.

Using R2 does **not** make the system automatically compliant with LGPD. See [LGPD.md](../security/LGPD.md).

## Cloudflare references

- [R2 S3 setup](https://developers.cloudflare.com/r2/get-started/s3/)
- [R2 API tokens](https://developers.cloudflare.com/r2/api/tokens/)
- [Pre-signed URL model](https://developers.cloudflare.com/r2/api/s3/presigned-urls/)
- [R2 metrics and analytics](https://developers.cloudflare.com/r2/platform/metrics-analytics/)
- [R2 pricing](https://developers.cloudflare.com/r2/pricing/)
