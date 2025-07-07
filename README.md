# EduSuite

EduSuite is a comprehensive multi-tenant educational management system built with .NET 8, Blazor Server, and Entity Framework Core. It provides a robust platform for managing educational institutions with support for multiple tenants, user management, and scalable architecture.

## Features

- **Multi-Tenant Architecture**: Isolated data and configurations per tenant
- **Tenant Management**: Create, update, and manage multiple educational institutions
- **User Management**: Role-based access control and user administration
- **Modern UI**: Responsive Blazor Server interface with Bootstrap
- **API-First Design**: RESTful APIs with comprehensive documentation
- **Identity Management**: Secure authentication and authorization
- **Scalable Architecture**: Built with microservices principles

## Technology Stack

- **Backend**: .NET 8, ASP.NET Core, Entity Framework Core
- **Frontend**: Blazor Server, Bootstrap 5
- **Database**: SQL Server with Entity Framework migrations
- **Identity**: ASP.NET Core Identity with JWT tokens
- **Architecture**: Clean Architecture with Domain-Driven Design principles
- **Containerization**: Docker support for easy deployment

## Prerequisites

- .NET 8.0 SDK
- SQL Server (local instance or Docker)
- Docker Desktop (for containerized setup)
- Visual Studio 2022 or Visual Studio Code

## Getting Started

### Option 1: Quick Start with Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EduSuite
   ```

2. **Start the database**
   ```bash
   docker-compose up -d
   ```

3. **Wait for SQL Server to be ready**
   ```bash
   docker-compose ps
   ```
   Wait until the status shows as "healthy".

4. **Apply database migrations**
   ```bash
   cd src/EduSuite.ApiService
   dotnet ef database update --project ../EduSuite.Database
   ```

5. **Run the application**
   ```bash
   cd ../..
   dotnet run --project src/EduSuite.AppHost
   ```

6. **Access the application**
   - Web Application: `https://localhost:7001`
   - API Documentation: `https://localhost:7002/swagger`

### Option 2: Local SQL Server Setup

1. **Configure connection strings**
   Update `appsettings.Development.json` in both `EduSuite.ApiService` and `EduSuite.Identity` projects with your SQL Server connection string.

2. **Run database migrations**
   ```bash
   cd src/EduSuite.ApiService
   dotnet ef database update --project ../EduSuite.Database
   ```

3. **Start the application**
   ```bash
   cd ../..
   dotnet run --project src/EduSuite.AppHost
   ```

## Project Structure

```
EduSuite/
├── src/
│   ├── EduSuite.AppHost/          # Application orchestration
│   ├── EduSuite.ApiService/       # Main API service
│   ├── EduSuite.Database/         # Data access layer
│   ├── EduSuite.Identity/         # Identity service
│   ├── EduSuite.Web/             # Blazor Server web application
│   └── EduSuite.ServiceDefaults/ # Shared service configuration
├── tests/
│   ├── EduSuite.Tests/           # Unit and integration tests
│   └── EduSuite.Database.Tests/  # Database tests
├── docs/                         # Documentation
└── docker-compose.yml           # Docker configuration
```

## Database Connection Details (Docker Setup)

- **Server**: localhost,1433
- **Username**: sa
- **Password**: EduSuite123!
- **Database**: EduSuite (API) / EduSuite.Identity (Identity Service)

## Development

### Running Tests
```bash
dotnet test
```

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add <MigrationName> --project src/EduSuite.Database --startup-project src/EduSuite.ApiService

# Apply migrations
dotnet ef database update --project src/EduSuite.Database --startup-project src/EduSuite.ApiService
```

### API Documentation
The API includes Swagger documentation available at `/swagger` when running in development mode.

## Docker Management

### Stop the containers
```bash
docker-compose down
```

### Stop and remove all data
```bash
docker-compose down -v
```

### View container logs
```bash
docker-compose logs sqlserver
```

### Access SQL Server directly
```bash
docker exec -it edusuite-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'EduSuite123!'
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Run tests to ensure everything works
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For questions or support, please create an issue in the GitHub repository. 