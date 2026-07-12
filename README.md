# Care Guide (Backend)

## Project Goal

This solution centralizes all information relevant to the treatment of oncology patients in a single
place. This includes data on doctors and specialists, the list of medications in use, exams performed
or scheduled, surgeries and medical procedures, as well as the addresses and coordinates of hospitals
and clinics visited. It is also possible to track the evolution of symptoms and details of the
treatments received, giving the patient a complete and organized view of their care journey.

## Key Features

- **Manage Users**: Registration of information and login;
- **Manage People**: Registration of all information related to the person/user;
- **Manage Health**: Blood type, diseases, family history and clinical data;
- **Manage Doctors**: Registration of doctors, specialties, phones and addresses;
- **Manage Annotations**: Registration of important annotations;
- **Calendar/Events**: Appointments, exams, surgeries and medications, with addresses and file attachments;
- **Files**: Attach documents (e.g. reports and exams) to events.

## Technologies

- .NET 10;
- Entity Framework;
- PostgreSQL;
- Docker;

## Documentation

Full documentation index: [docs/README.md](docs/README.md).

| Topic | Document |
| ----- | -------- |
| Conventions and architecture (AI agents) | [AGENTS.md](AGENTS.md) |
| Solution structure | [docs/architecture/STRUCTURE.md](docs/architecture/STRUCTURE.md) |
| Execution | [docs/getting-started/EXECUTION.md](docs/getting-started/EXECUTION.md) |
| Configuration and secrets | [docs/getting-started/CONFIGURATION.md](docs/getting-started/CONFIGURATION.md) |
| Private R2 file flow | [docs/architecture/FILE_STORAGE.md](docs/architecture/FILE_STORAGE.md) |
| R2 operations runbook | [docs/operations/R2_RUNBOOK.md](docs/operations/R2_RUNBOOK.md) |
| API conventions | [docs/architecture/API.md](docs/architecture/API.md) |
| Migrations | [docs/development/MIGRATIONS.md](docs/development/MIGRATIONS.md) |
| Development database seed | [database/seeds/README.md](database/seeds/README.md) |
| Tests | [docs/development/TESTS.md](docs/development/TESTS.md) |
| Security | [docs/security/SECURITY.md](docs/security/SECURITY.md) |
| LGPD / Privacy | [docs/security/LGPD.md](docs/security/LGPD.md) |
| ER Diagram | <a href="https://miro.com/app/board/uXjVNh5qees=/" target="_blank" rel="noopener noreferrer">ER Diagram (Miro)</a> |

## API Documentation (OpenAPI + Scalar)

The API is documented using the **OpenAPI** standard and the interactive **Scalar** interface.

When running the application, the documentation will be available at:

- `http://localhost:<port>/scalar`

This interface lets you view all endpoints, request/response schemas, examples, and run tests directly
from the browser.

## Project Members

- [Amanda Maschio](https://www.linkedin.com/in/amanda-maschio/)
- [Fernando Waldow Martens](https://www.linkedin.com/in/fernandomartens/)
