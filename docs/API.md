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

- **Bearer JWT** scheme in the `Authorization: Bearer <token>` header.
- The `/api/v1` group requires authentication (`RequireAuthorization`).
- Anonymous endpoints are marked with `AllowAnonymous` (e.g. create account, login, refresh).
- Token and cookie details in [SECURITY.md](SECURITY.md).

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

`page`/`pageSize` are normalized by `BaseRepository`/`BasePersonOwnedRepository`, never throw: `page < 1` → `DefaultPage`; `pageSize < 1` → `DefaultPageSize`; `pageSize > MaxPageSize` → `MaxPageSize`.

## Validation

Automatic via `ValidationFilterFactory` — do not call the validator manually in the endpoint.
Every input DTO has a matching validator. See [AGENTS.md](../AGENTS.md) → Validators.

## Field limits

| Constant | Value |
| -------- | ----- |
| `DatabaseConstants.MaxLengthStandardText` | 255 |
| `DatabaseConstants.MaxLengthLargeText` | 1000 |

---

[Documentation index](README.md) · [Project README](../README.md)
