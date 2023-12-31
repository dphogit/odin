# Odin - Home Electronics Management

A simple home electronics management system intended for personal and local use.

## 📚 Motivation

I've been getting into electronics and want to build some simple home devices. For example. measuring the temperature in my room with an Arduino and TMP36 sensor, or using a Raspberry Pi to send temperature/humidity data over WiFi. I want to be able to manage these devices and view/store this collected data.

## ⚙ Development Setup

### Database

Create an .env file in the root of the project and set the following variables:

```env
MSSQL_SA_PASSWORD=
```

Run `docker compose up -d` which sets up a SQL Server 2022 database container. When started, create a database named `Odin`. The following steps use the command line, but you can also follow other methods such as SSMS to create the database. Refer to the [documentation](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker?view=sql-server-ver16&pivots=cs1-bash#connect-to-sql-server) for help during this process or other methods.

First, start an interactive bash shell of the db container:

```bash
docker exec -it sql1 bash
```

Once inside the container, connect locally with `sqlcmd`:

```bash
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $MSSQL_SA_PASSWORD
```

If successful, you should get to a sqlcmd command prompt: `1>`. Now create the database:

```sql
CREATE DATABASE Odin;
SELECT Name from sys.Databases; -- (Optional) outputs the list of databases which you can verify
GO
```

You can exit out of sqlcmd with `quit` and then exit the container interactive shell with `exit`.

This project uses migrations to manage the database schema. Apply the migrations:

```bash
dotnet ef database update --project Odin.Api
```

That's the database setup! Some side notes:

- You have System Administrator (SA) privileges on this local database, fine for local development but bad practice for production and rather you should disable the SA account and create a user with minimum privileges required.
- The password you set in `MSQL_SA_PASSWORD` is the password for the SA user, which you can currently view through a simple `echo $MSSQL_SA_PASSWORD` command. Change this through the `ALTER LOGIN` command if you don't want the SA password to be accessible via your machine's environment variables.

### REST API

For the API to connect to the database, dotnet [user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows) are used.

Set a secret for the connection string (replace `<YOUR_PASSWORD>` with the password you set in the .env file):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection;Server=localhost,1433;User=SA;Password=<YOUR_PASSWORD>;TrustServerCertificate=True;Database=odin --project Odin.Api
```

Run the API:

```bash
dotnet run --project Odin.Api
```

### Web App

The frontend is a React-TypeScript app which uses [Vite](https://vitejs.dev/). As of Vite 5.0, Vite requires Node.js version 18+. 20+. SOme dependencies may require later versions so upgrade with your package manager if you receive any warnings.

Install dependencies:

```bash
npm install
```

Run the app for development (Fast Refresh enabled):

```bash
npm run dev
```

Access the app at <http://localhost:3000>.

## 🧪 Testing

### Integration Tests (Odin.API)

```bash
dotnet test --filter FullyQualifiedName~Odin.Api.IntegrationTests
```

Integration tests are using Xunit, testcontainers and respawn. At the start of the integration tests for an XUnit defined collection, a docker container is started with the same SQL Server 2022 image as the API is using then reset to a clean state at the end of each test.

## Samples

The [Samples](/Samples) folder contains examples of clients that can be used to interact with this system.
