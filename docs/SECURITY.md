# Security

Overview of the Care Guide API security mechanisms. All referenced code lives in `CareGuide.Security`
and `CareGuide.API` (middlewares, filters and helpers).

See also: [LGPD.md](LGPD.md) · [CONFIGURATION.md](CONFIGURATION.md) · [API.md](API.md)

---

## 1. Authentication

Flow based on **JWT (access token)** + **refresh token**.

| Item | Value | Where |
| ---- | ----- | ----- |
| Signing algorithm | HMAC-SHA512 | `JwtService.GenerateToken` |
| Access token expiry | 15 minutes | `JwtService` |
| Claims | `sub` (userId), `personId`, `role`, `jti` | `JwtService` |
| Refresh token | 32 random bytes (RNG), Base64 | `JwtService.GenerateRefreshToken` |
| Rotation / invalidation | Yes | `RefreshTokenService` |

### Session cookies (`AuthCookieHelper`)

Both the `sessionToken` (access, 15 min) and the `refreshToken` (7 days) are written as cookies:

- `HttpOnly = true` — not accessible from JavaScript.
- `Secure = true` — only sent over HTTPS.
- `SameSite = Strict` — CSRF protection.

On successful login, the JSON response contains only account profile data. Tokens are not included in
the response body; they are delivered through the cookies above. Non-browser API clients can send an
access token with `Authorization: Bearer <token>`; that header takes precedence over `sessionToken`.

The `POST /api/v1/accounts/refresh` endpoint reads the refresh token **from the cookie**, not from
the body, to avoid leaking it into client logs.

---

## 2. Passwords

Implemented in `PasswordManagerHelper`.

- **Hash:** ASP.NET Core Identity `PasswordHasher<T>` (PBKDF2). Verification supports rehash.
- **Minimum policy:** at least one uppercase, one lowercase, one digit and one special character.
- **Strength:** evaluated with **zxcvbn** — requires `Score >= 3` to accept the password.

---

## 3. Rate limiting (`RateLimitingExtension`)

| Policy | Target | Limit |
| ------ | ------ | ----- |
| `general` | Versioned API endpoints under `/api/v1` | 60 req/min per authenticated user (sliding window); falls back to per-IP for anonymous requests |
| `auth` | `/api/v1/accounts` create, login and refresh | 5 req/min per IP (fixed window) — anti brute-force |

Account endpoints are registered under `/api/v1/accounts`. Create, login and refresh are anonymous
and explicitly use the stricter `auth` policy; protected account endpoints remain under the versioned
API group and require authentication. The endpoint-specific `auth` policy is the intended limiter for
those three authentication routes.

The `general` policy partitions authenticated traffic by the validated JWT `sub` (userId) claim rather
than IP, so users sharing a public IP (e.g. mobile carrier CGNAT) do not throttle each other. The
partition key is never taken from a client-supplied header. Genuinely anonymous requests to `general`
endpoints, and all `auth`-policy requests, still partition by IP, resolved considering
`X-Forwarded-For` (behind a proxy) — preserving brute-force protection on the authentication routes.

Response when exceeded: **HTTP 429** with `Retry-After` and an `application/problem+json` body.

---

## 4. Security headers (`SecurityHeadersMiddleware`)

Applied to every response:

- `X-Content-Type-Options: nosniff`
- `X-Frame-Options: DENY`
- `Referrer-Policy: no-referrer`

In production (`Program.cs`): `UseHsts()` + `UseHttpsRedirection()`.

---

## 5. Per-owner data isolation

Entities tied to the logged-in user implement `IPersonOwnedEntity` and are accessed through
`IBasePersonOwnedRepository`, which restricts queries to the session's `PersonId`. This prevents a
user from reading/editing another user's data. See [AGENTS.md](../AGENTS.md) → Repositories.

---

## 6. CORS

Allowed origins configured in `CorsOrigins` (`appsettings`). See [CONFIGURATION.md](CONFIGURATION.md).

---

## 7. Secret management

The JWT `SecretKey` and the connection string **must not be versioned** with real values.
In the repository these fields are empty; provide the values locally/in production via User Secrets,
environment variables or a secrets vault. See [CONFIGURATION.md](CONFIGURATION.md) §Secrets.

### Private R2 attachments

- The R2 bucket remains private; no public bucket URL or public custom domain is part of the design.
- Permanent R2 credentials exist only in server configuration and are never returned to clients.
- Pre-signed URLs authorize one method on one opaque object key for a configured short lifetime.
- Event, attachment and file queries are person-owned; possessing a `fileId` or storage key is insufficient.
- Upload size, extension and MIME type use validated allowlists. Client filenames are metadata only and
  are sanitized before being placed in signed download `Content-Disposition`.
- Confirmation checks R2 object metadata. `ETag` is not assumed to be a SHA-256 content digest.
- URLs, credentials, object keys and patient filenames must not appear in application logs or error bodies.
- Cleanup and definitive deletion preserve referenced objects and expose partial failures for retry.

Operational controls and credential rotation are documented in [R2_RUNBOOK.md](R2_RUNBOOK.md).

---

## 8. Reporting vulnerabilities

Do not open a public issue for security flaws. Contact a maintainer directly
(see [../README.md](../README.md) → Project Members).

---

[Documentation index](README.md) · [Project README](../README.md)
