# LGPD Compliance

> **Scope:** This document describes how the **Care Guide API** processes personal data under
> **Law No. 13.709/2018 (Lei Geral de Proteção de Dados Pessoais — LGPD)**, the Brazilian General Data
> Protection Law. It serves as a technical reference for the team and as a basis for data-subject-facing
> policies.
>
> **Notice:** This is a technical engineering document and **does not replace legal advice**.
> Items marked as _pending_ represent measures not yet implemented and must be addressed before any
> production use with real data.

See also: [AGENTS.md](../AGENTS.md) · [SECURITY.md](SECURITY.md) · [STRUCTURE.md](STRUCTURE.md)

---

## 1. Why the LGPD applies

Care Guide centralizes the care journey of oncology patients. This involves processing **personal data**
and, above all, **sensitive personal data** (art. 5, II of the LGPD), since it includes data
**concerning health**. Sensitive data has reinforced protection and its own processing rules (art. 11).

---

## 2. Personal data processed

Mapped from the entities in `src/CareGuide.Models/Entities/`.

### 2.1 Identification and account data

| Data | Entity | Classification |
| ---- | ------ | -------------- |
| Name | `Person.Name` | Personal |
| Gender | `Person.Gender` | Personal |
| Date of birth | `Person.Birthday` | Personal |
| Picture | `Person.Picture` | Personal |
| Email | `User.Email` | Personal |
| Password (hash) | `User.Password` | Personal (credential) |
| Phones | `PersonPhone` / `Phone` | Personal |

### 2.2 Sensitive health data (art. 11)

| Data | Entity | Classification |
| ---- | ------ | -------------- |
| Blood type, height, weight, clinical description | `PersonHealth` | **Sensitive — health** |
| Diseases and diagnoses | `PersonDisease` | **Sensitive — health** |
| Family health history | `PersonFamilyHistory` | **Sensitive — health** (includes third-party data) |
| Clinical annotations | `PersonAnnotation` | **Sensitive — health** |
| Events: appointments, exams, surgeries, medications | `Event*` | **Sensitive — health** |
| Attached files (e.g. reports, exams) | `FileAsset` / `EventFile` | **Sensitive — health** |
| Doctors and specialties linked to the patient | `Doctor*` | Personal (third-party) + reveals health condition |

> **Attention — third-party data:** `PersonFamilyHistory` and `Doctor` store data about people who
> are **not the account holder**. The holder acts as the source, but those third parties are also
> data subjects with rights. Evaluate the legal basis and data minimization.

---

## 3. Legal bases (art. 7 and art. 11)

| Purpose | Suggested legal basis | Note |
| ------- | --------------------- | ---- |
| Account creation and authentication | Contract performance (art. 7, V) | — |
| Processing of health data | **Specific and highlighted consent** (art. 11, I) OR protection of health (art. 11, II, "f") | For sensitive data, generic consent is **not enough** |
| Operational communications | Legitimate interest / contract performance | Evaluate case by case |

**Pending:** implement consent recording (date, terms version, purpose) as evidence of compliance.
There is currently no consent entity in the model.

---

## 4. Data subject rights (art. 18)

| Right | Current support | Endpoint / mechanism |
| ----- | --------------- | -------------------- |
| Confirmation and access | Partial | `GET` of Person/PersonHealth/etc. via authenticated API |
| Correction | Yes | `PUT`/`PATCH` of the holder's entities |
| Deletion | Partial | `DELETE /api/v1/accounts/{id}` — **see §6 on soft vs hard delete** |
| Portability | _Pending_ | No structured data export |
| Information on data sharing | _Pending_ | Document third parties/processors |
| Consent revocation | _Pending_ | Depends on consent recording (§3) |

---

## 5. Technical security measures (art. 46)

Implemented today (details in [SECURITY.md](SECURITY.md)):

- **Authentication:** JWT (HMAC-SHA512), 15-minute expiry; refresh token in a cookie.
- **Passwords:** PBKDF2 hash (ASP.NET Identity); strength evaluated with **zxcvbn** at registration.
- **Refresh token:** rotation and invalidation (`RefreshTokenService`); `HttpOnly` cookie.
- **Rate limiting** on authentication endpoints (`AuthPolicy`).
- **Per-owner isolation:** `IBasePersonOwnedRepository` restricts data to the logged-in `PersonId`.

**Pending / to validate:**

- [ ] Encryption at rest of sensitive data (column or disk).
- [ ] Enforced TLS/HTTPS in production (HSTS).
- [ ] Audit log of access to health data (who accessed what and when).
- [ ] Secrets (`SecretKey`, connection string) outside the versioned `appsettings` — use a vault/environment variables.
- [ ] Backup policy and incident response (art. 48 — notification to the ANPD).
- [ ] Anonymization/pseudonymization for test environments.

---

## 6. Retention and deletion

- **Basis:** `Entity` has an `IsActive` flag, suggesting **soft delete**. Verify whether
  `DELETE /api/v1/accounts/{id}` performs a logical or physical deletion.
- **LGPD requirement:** data must be deleted after the purpose ends (art. 15/16), except for legal
  retention cases. A soft delete does **not** by itself satisfy the right to deletion.
- **Pending:** define a retention period per data category and a purge routine (hard delete) for the
  holder who requests deletion, including files (`FileAsset`) and tokens.

---

## 7. Processors and third-party sharing

Inventory and document every external service that processes personal data (art. 39):

- Hosting / database provider (managed PostgreSQL?).
- File storage (`FileAsset` — where are the binaries kept?).
- Any email, monitoring or analytics service.

**Pending:** keep processor agreements (DPA) with each third party.

---

## 8. Compliance checklist

- [ ] Specific consent recording for health data
- [ ] Privacy policy accessible to the data subject
- [ ] Data export (portability)
- [ ] Definitive deletion routine on subject request
- [ ] Encryption at rest of sensitive data
- [ ] Mandatory HTTPS/HSTS in production
- [ ] Audit log of sensitive-data access
- [ ] Secrets outside the repository
- [ ] Processor inventory + DPAs
- [ ] Incident response plan and ANPD notification
- [ ] Data Protection Officer (DPO) designation, if applicable (art. 41)

---

## 9. References

- [Law No. 13.709/2018 (LGPD) — Planalto](http://www.planalto.gov.br/ccivil_03/_ato2015-2018/2018/lei/l13709.htm)
- [ANPD — National Data Protection Authority](https://www.gov.br/anpd/pt-br)
- [Information security guide for processing agents — ANPD](https://www.gov.br/anpd/pt-br/documentos-e-publicacoes)

---

[Documentation index](README.md) · [Project README](../README.md)
