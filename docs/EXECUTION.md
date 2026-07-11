# Application Execution Guide

> Configuration (ports, environment variables and secrets): see [CONFIGURATION.md](CONFIGURATION.md).

### Prerequisites

- [Docker Desktop](https://docs.docker.com/get-docker/) installed and running.
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or later installed (for local execution via IIS Express).

### Step by Step

1. **Clone the repository:**

   ```sh
   git clone https://github.com/care-guide/care-guide-api.git
   ```

2. **Check the configuration files:**  
   Make sure the `Dockerfile` and `docker-compose.yml` files are present at the project root.

3. **Create the `.env` file:**  
   `docker-compose` reads database credentials and ports from a `.env` file at the
   project root (used to substitute `${VAR}` in `docker-compose.yml`). It is gitignored,
   so it does not exist after a fresh clone. Copy the template and adjust as needed:

   ```sh
   cp .env.example .env
   ```

   Without this file, the `postgres` container starts with blank user/password and the
   healthcheck fails, so `careguide_db` becomes unhealthy and the API does not start.
   See [CONFIGURATION.md](CONFIGURATION.md) for what each variable does.

### Running via Docker Compose

4. **Start the application with Docker Compose:**

   ```sh
   docker compose up -d
   ```

   All required services will be configured and started automatically.

5. **Apply pending migrations from the repository root:**

   ```sh
   dotnet ef database update --project src/CareGuide.Infra --startup-project src/CareGuide.API
   ```

   If the design-time factory does not receive your local connection string, pass it explicitly with
   `--connection`. See [MIGRATIONS.md](MIGRATIONS.md) and [CONFIGURATION.md](CONFIGURATION.md).

6. **Populate development data (optional):**

   ```sh
   dotnet run --project database/seeds/CareGuide.Seeds.csproj -- --env-file .env
   ```

   This creates complete, isolated datasets for a normal user and an administrator. See the
   [development seed guide](../database/seeds/README.md) for credentials and rerun behavior.

### Important Information

- The API will be available at: [http://localhost:8080](http://localhost:8080)
- The PostgreSQL database uses port `5434`.
- Make sure ports `8080` and `5434` are free before starting the project.

### Useful Commands

- **Recreate containers after changes:**

  ```sh
  docker compose up -d --build
  ```

- **Stop running containers:**

  ```sh
  docker compose stop
  ```

- **Stop and remove containers:**
  ```sh
  docker compose down
  ```

---

### Running Locally with IIS Express

1. Open the `CareGuide.slnx` solution in Visual Studio.
2. Select `CareGuide.API` as the startup project.
3. In the top menu, select the **IIS Express** profile.
4. Press `F5` or click **Start Debugging** to run the API locally.
5. The API will be available at `http://localhost:5000` (or the port configured in `launchSettings.json`).

   > Make sure the database is accessible and correctly configured in `appsettings.Development.json`.

---

[Documentation index](README.md) · [Project README](../README.md)
