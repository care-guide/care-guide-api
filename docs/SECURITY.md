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

The `POST /accounts/refresh` endpoint reads the refresh token **from the cookie**, not from the body,
to avoid leaking it into client logs.

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
| `general` | All authenticated endpoints | 60 req/min per IP (sliding window) |
| `auth` | `/accounts` (login, refresh, create) | 5 req/min per IP (fixed window) — anti brute-force |

Response when exceeded: **HTTP 429** with `Retry-After` and an `application/problem+json` body.
The IP is resolved considering `X-Forwarded-For` (behind a proxy).

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

---

## 8. Reporting vulnerabilities

Do not open a public issue for security flaws. Contact a maintainer directly
(see [../README.md](../README.md) → Project Members).

---

[Documentation index](README.md) · [Project README](../README.md)
