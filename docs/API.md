# API Conventions

HTTP contract standards of the Care Guide API. For authentication and security, see [SECURITY.md](SECURITY.md);
to create endpoints, see [AGENTS.md](../AGENTS.md) → Endpoints.

---

## Base and versioning

- Prefix: `/api` (`ApiConstants.API_PREFIX`)
- Version: `/v1` (`ApiConstants.API_VERSION`)
- Effective base route: **`/api/v1/...`**

Account endpoints also live under the versioned base route: **`/api/v1/accounts`**.
Account creation, login and refresh are explicitly marked as anonymous; logout, password update
and account deletion require authentication.

## Interactive documentation

- **OpenAPI** generated automatically.
- **Scalar** available at `http://localhost:<port>/scalar` (**Development only**).

## Authentication

- Browser clients receive the access token in the `sessionToken` **HttpOnly cookie** after login.
- API clients may alternatively use the **Bearer JWT** scheme in the
  `Authorization: Bearer <token>` header. When both are present, the header takes precedence.
- The `/api/v1` group requires authentication (`RequireAuthorization`).
- Anonymous endpoints are marked with `AllowAnonymous` (e.g. create account, login, refresh).
- Token and cookie details in [SECURITY.md](SECURITY.md).

Successful login returns account profile fields (`id`, `email`, `name`, `gender`, `birthday`) in the
JSON body. Access and refresh tokens are written to HttpOnly cookies and are not returned in that body.

## Error format — RFC 9457 (ProblemDetails)

Every error returns `application/problem+json` via `ErrorHandlerMiddleware`, following
[RFC 9457](https://www.rfc-editor.org/rfc/rfc9457.html) (`ProblemDetails`, successor to RFC 7807).
Structure:

```json
{
  "type": "https://problems-registry.smartbear.com/validation-error",
  "title": "Validation failed.",
  "status": 400,
  "detail": "One or more fields are invalid.",
  "instance": "/api/v1/doctors",
  "errors": {
    "Name": ["'Name' must not be empty."]
  }
}
```

Exception → status mapping (`ErrorHandlerMiddleware`):

| Exception | Status |
| --------- | ------ |
| `ValidationException` (FluentValidation) | 400 (with `errors`) |
| `ArgumentException` / `ArgumentNullException` | 400 |
| `InvalidOperationException` / `HttpRequestException` | 400 |
| `KeyNotFoundException` | 404 |
| `UnauthorizedAccessException` | 401 |
| `DbUpdateConcurrencyException` | 409 |
| `DbUpdateException` | 400 |
| `TimeoutException` | 504 |
| `NotImplementedException` | 501 |
| Others | 500 |

> `stackTrace`/`traceId` appear in `extensions` **only in Development**.

## Rate limiting

429 (`Too many requests`) with a `Retry-After` header. Limits in [SECURITY.md](SECURITY.md) §3.

## Pagination

Constants in `PaginationConstants`:

| Constant | Value |
| -------- | ----- |
| `DefaultPage` | `1` |
| `DefaultPageSize` | `10` |
| `MaxPageSize` | `100` |

`page`/`pageSize` are normalized (never throw) by `PaginationNormalizer.Normalize`, reused by
`BaseRepository`/`BasePersonOwnedRepository` and by the join-table repositories (DoctorAddress,
DoctorPhone, DoctorSpecialty, PersonPhone, EventAddress, EventFile): `page < 1` → `DefaultPage`;
`pageSize < 1` → `DefaultPageSize`; `pageSize > MaxPageSize` → `MaxPageSize`.

### Envelope

Every collection endpoint returns the same envelope, `PagedResult<T>`
(`CareGuide.Models/DTOs/Shared/PagedResult.cs`):

```json
{
  "items": [ /* T[] */ ],
  "page": 1,
  "pageSize": 10,
  "total": 42,
  "hasNext": true
}
```

- `total` is always a real `COUNT` query scoped with the same person/parent-resource predicate as
  `items` (never `items.Count` or a constant), run alongside the paged query — no N+1, no
  full-table load.
- `hasNext` is computed as `page * pageSize < total`.
- Applies uniformly to doctors, doctor sub-resources (addresses/phones/specialties), events, event
  sub-resources (addresses/files), and all `person-*` resources. Singular per-event resources
  (appointment, exam, medicine, surgery — one record per event) are not paginated; they are not
  collections.

## Validation

Automatic via `ValidationFilterFactory` — do not call the validator manually in the endpoint.
Every input DTO has a matching validator. See [AGENTS.md](../AGENTS.md) → Validators.

## Direct-upload integrity

Direct event-file uploads require the client to send the expected SHA-256 digest as the signed
`x-amz-meta-sha256` header. Confirmation performs an R2 `HEAD` request and compares this metadata,
the content length, and the content type with the pending `FileAsset` before attaching it.

R2 does not expose a universally reliable SHA-256 content digest through `ETag`; multipart ETags in
particular must not be interpreted as hashes. The signed metadata prevents the confirmation request
from silently changing the declared digest, but it does not independently prove the bytes' digest.
If cryptographic verification of stored bytes becomes mandatory, a trusted backend process must
stream the object and calculate SHA-256 (or use a future provider checksum with documented semantics)
before changing the upload status.

## Private event-file flow

The complete request/response examples are in [FILE_STORAGE.md](FILE_STORAGE.md). All routes are
authenticated and use the `/api/v1` prefix:

| Step | Route | Success | Common failures |
| ---- | ----- | ------- | --------------- |
| Initiate | `POST /events/{eventId}/files/uploads` | `201` pending file + pre-signed `PUT` | `400` invalid metadata; `404` event outside scope |
| Confirm | `POST /events/{eventId}/files/{fileId}/confirm` | `200` idempotent attachment | `400` mismatch/state; `404` record/object missing |
| Download | `GET /events/{eventId}/files/{fileId}/download` | `200` temporary GET URL | `400` pending; `404` unauthorized/missing object |
| Detach | `DELETE /events/{eventId}/files/{fileId}` | `204` | `404` association outside scope/missing |
| Delete | `DELETE /files/{fileId}` | `204` | `400` still referenced/storage failure; `404` outside scope/missing |

Pre-signed URLs are opaque temporary capabilities. Clients must not log, persist, cache beyond expiry,
modify their query parameters, or reuse a PUT URL for GET (and vice versa).

## Field limits

| Constant | Value |
| -------- | ----- |
| `DatabaseConstants.MaxLengthStandardText` | 255 |
| `DatabaseConstants.MaxLengthLargeText` | 1000 |

---

[Documentation index](README.md) · [Project README](../README.md)
