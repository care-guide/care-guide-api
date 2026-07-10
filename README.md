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

## Database Entity-Relationship Diagram

- [ER Diagram](https://miro.com/app/board/uXjVNh5qees=/)

## Documentation

Full documentation index: [docs/README.md](docs/README.md).

| Topic | Document |
| ----- | -------- |
| Conventions and architecture (AI agents) | [AGENTS.md](AGENTS.md) |
| Solution structure | [docs/STRUCTURE.md](docs/STRUCTURE.md) |
| Execution | [docs/EXECUTION.md](docs/EXECUTION.md) |
| Configuration and secrets | [docs/CONFIGURATION.md](docs/CONFIGURATION.md) |
| API conventions | [docs/API.md](docs/API.md) |
| Migrations | [docs/MIGRATIONS.md](docs/MIGRATIONS.md) |
| Tests | [docs/TESTS.md](docs/TESTS.md) |
| Security | [docs/SECURITY.md](docs/SECURITY.md) |
| LGPD / Privacy | [docs/LGPD.md](docs/LGPD.md) |

## Structure

For more information about the structure of the solution, see the following [documentation](docs/STRUCTURE.md).

## Execution

For a step-by-step guide on how to run the application, see the following [documentation](docs/EXECUTION.md).

## Migrations

To learn how to create and run migrations in the project, see the following [documentation](docs/MIGRATIONS.md).

## Tests

For information about the unit test structure and how to run it, see the following [documentation](docs/TESTS.md).

## API Documentation (OpenAPI + Scalar)

The API is documented using the **OpenAPI** standard and the interactive **Scalar** interface.

When running the application, the documentation will be available at:

- `http://localhost:<port>/scalar`

This interface lets you view all endpoints, request/response schemas, examples, and run tests directly
from the browser.

## Project Members

- [Amanda Maschio](https://www.linkedin.com/in/amanda-maschio/)
- [Fernando Waldow Martens](https://www.linkedin.com/in/fernandomartens/)
