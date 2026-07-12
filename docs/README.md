# Documentation — Care Guide API

Central documentation index for the project. Start here.

## Getting started

| Document | What it covers |
| -------- | -------------- |
| [../README.md](../README.md) | Project overview, goal and features |
| [getting-started/EXECUTION.md](getting-started/EXECUTION.md) | How to run the application (Docker and local) |
| [getting-started/CONFIGURATION.md](getting-started/CONFIGURATION.md) | Environment variables, `appsettings`, ports and secrets |

## Development

| Document | What it covers |
| -------- | -------------- |
| [../AGENTS.md](../AGENTS.md) | Conventions and architecture guide (AI-agent context) |
| [development/MIGRATIONS.md](development/MIGRATIONS.md) | Create and apply EF Core migrations |
| [../database/seeds/README.md](../database/seeds/README.md) | Populate a local database with development data and test accounts |
| [development/TESTS.md](development/TESTS.md) | Unit test structure and execution |

## Architecture

| Document | What it covers |
| -------- | -------------- |
| [architecture/STRUCTURE.md](architecture/STRUCTURE.md) | Project and layer structure of the solution |
| [architecture/API.md](architecture/API.md) | REST conventions, errors (RFC 9457 ProblemDetails), pagination and auth |
| [architecture/FILE_STORAGE.md](architecture/FILE_STORAGE.md) | Complete private R2 upload, confirmation, download and deletion flow |

## Operations

| Document | What it covers |
| -------- | -------------- |
| [operations/R2_RUNBOOK.md](operations/R2_RUNBOOK.md) | Credential rotation, orphan cleanup, deletion failures and cost monitoring |

## Security and compliance

| Document | What it covers |
| -------- | -------------- |
| [security/SECURITY.md](security/SECURITY.md) | Authentication, passwords, rate limiting, headers and secrets |
| [security/LGPD.md](security/LGPD.md) | Handling of personal and sensitive data; LGPD compliance |
