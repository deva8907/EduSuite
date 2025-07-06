# EduSuite Docker Setup

This document provides instructions for running EduSuite with a containerized SQL Server database.

## Prerequisites

- Docker Desktop installed and running
- .NET 8.0 SDK installed

## Getting Started

### 1. Start SQL Server Container

From the root directory of the project, run:

```bash
docker-compose up -d
```

This will:
- Start a SQL Server 2022 container
- Expose SQL Server on port 1433
- Create a persistent volume for database data
- Set up health checks to ensure the database is ready

### 2. Wait for SQL Server to be Ready

The container includes health checks. You can monitor the status with:

```bash
docker-compose ps
```

Wait until the status shows as "healthy" before proceeding.

### 3. Run Database Migrations

Once SQL Server is running, you'll need to create and apply database migrations:

```bash
# Navigate to the API service directory
cd src/EduSuite.ApiService

# Create initial migration (if not already done)
dotnet ef migrations add InitialCreate --project ../EduSuite.Database

# Apply migrations to create the database schema
dotnet ef database update --project ../EduSuite.Database
```

### 4. Start the Application

You can now start the EduSuite application:

```bash
# From the root directory
dotnet run --project src/EduSuite.AppHost
```

## Database Connection Details

- **Server**: localhost,1433
- **Username**: sa
- **Password**: EduSuite123!
- **Database**: EduSuite (API) / EduSuite.Identity (Identity Service)

## Managing the Container

### Stop the SQL Server container:
```bash
docker-compose down
```

### Stop and remove all data:
```bash
docker-compose down -v
```

### View container logs:
```bash
docker-compose logs sqlserver
```

### Access SQL Server directly:
```bash
docker exec -it edusuite-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'EduSuite123!'
```

## Notes

- The database data is persisted in a Docker volume named `edusuite_sqlserver_data`
- The SA password is set to `EduSuite123!` for development purposes
- For production environments, use environment variables or secrets management for the password
- The `TrustServerCertificate=True` setting is used to bypass SSL certificate validation for development 