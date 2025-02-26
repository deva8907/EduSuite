# Student Management System - Technical Specification

## 1. System Overview
### 1.1 Purpose
A multi-tenant student management system for Indian schools (Kindergarten to Class XII) with initial focus on CBSE board requirements while maintaining flexibility for other boards.

### 1.2 Target Users
- School Administrators
- Teachers
- Administrative Staff
- Parents/Guardians
- Students (future consideration)

### 1.3 System Architecture Overview
- .NET Aspire Application Host for Service Orchestration
- Blazor Web Server (Frontend Application)
- Backend API Server
- Dedicated Identity Service
- Separate Identity Database
- Application Database
- Shared Library for Common Code

## 2. Technical Stack
### 2.1 Frontend
- Blazor Server
- SignalR (built into Blazor Server)
- Component Library
- HTTP Client for API communication

### 2.2 Backend
- .NET 8 Web API
- .NET Aspire for service orchestration and monitoring
- Duende IdentityServer (as separate service)
- Entity Framework Core
- SQL Server
- OpenTelemetry for observability

## 3. Solution Architecture
### 3.1 Vertical Slice Architecture with Aspire Integration
```
Solution/
├── src/
│   ├── EduSuite.AppHost/             # Orchestration Host
│   │   ├── Program.cs              # Service registration
│   │   └── appsettings.json        # Environment configuration
│   │
│   ├── EduSuite.ServiceDefaults/     # Shared Service Configuration
│   │   ├── Extensions/             # Service collection extensions
│   │   └── OpenTelemetry/          # Telemetry configuration
│   │
│   ├── EduSuite.Web/                 # Blazor Web Server
│   │   ├── Features/
│   │   │   ├── Students/           
│   │   │   ├── Staff/             
│   │   │   ├── Attendance/
│   │   │   └── Fees/
│   │   ├── Shared/                
│   │   └── Infrastructure/        
│   │
│   ├── EduSuite.Api/                       # Main API Service
│   │   ├── Features/
│   │   │   ├── Students/          
│   │   │   ├── Staff/            
│   │   │   ├── Attendance/
│   │   │   └── Fees/
│   │   └── Infrastructure/        
│   │
│   ├── EduSuite.Identity/              # Dedicated Identity Service
│       ├── Configuration/         # IdentityServer configuration
│       ├── Data/                  
│       ├── Models/
│       ├── Services/
│       └── Controllers/
│   
│   
│
└── tests/
    ├── Features/                  
    │   ├── Students.Tests/
    │   └── Staff.Tests/
    └── Infrastructure.Tests/
```

### 3.2 Feature Module Structure (Example: Student Feature)
```
Students/
├── Commands/
│   ├── CreateStudent/
│   └── UpdateStudent/
├── Queries/
│   ├── GetStudentList/
│   └── GetStudentDetails/
├── Models/
├── Validators/
└── Infrastructure/
```

## 4. Authentication & Authorization
### 4.1 Identity Architecture
- OpenID Connect (OIDC) with OAuth 2.0
- Duende IdentityServer implementation as separate service
- Separate identity database
- Service-to-service authentication

### 4.2 Identity Database Schema
```sql
-- Separate database: SMS.Identity
-- Tables include:
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- IdentityServerConfiguration
- PersistedGrants
```

### 4.3 Authentication Flows
- Authorization Code with PKCE for Blazor Server
- JWT tokens for API authentication
- Refresh token support
- Multi-tenant user management
- Service-to-service authentication using client credentials

## 5. Multi-tenancy
### 5.1 Tenant Structure
- School (Tenant)
- Multiple sites per school
- Tenant-specific configurations
- Site-specific settings

### 5.2 Data Isolation
- Shared database with tenant ID in tables
- Row-level security
- Tenant context middleware
- Database sharding support through Aspire configuration

## 6. First Iteration Features
### 6.1 Core Features
1. Tenant/School Setup
   - Registration
   - Site management
   - Configuration
   - Service orchestration dashboard

2. Student Management
   - Registration
   - Profile management
   - Document handling
   - Class allocation

### 6.2 Technical Requirements
1. Authentication & Authorization
2. Multi-tenant infrastructure
3. Basic reporting
4. Document storage
5. Service health monitoring
6. Distributed tracing

## 7. Database Strategy
### 7.1 Multiple Databases
1. Application Database (SMS)
   - All application data
   - Tenant-specific data
   - Transactional data

2. Identity Database (SMS.Identity)
   - User management
   - Authentication data
   - Authorization data

### 7.2 Migration Strategy
- Separate migrations for each database
- Tenant-aware migrations
- Version control for schema changes
- Database deployment through Aspire

## 8. API Design
### 8.1 REST Endpoints
```
/api/v1/tenants
/api/v1/students
/api/v1/documents
/api/v1/classes
```

### 8.2 Response Format
```json
{
  "success": boolean,
  "data": object,
  "message": string,
  "errors": array
}
```

## 9. Security Considerations
### 9.1 Data Protection
- Encryption at rest
- Secure service-to-service communication
- Document storage security
- PII data handling
- Secrets management through Azure KeyVault

### 9.2 Authentication Security
- JWT token security
- Refresh token rotation
- Session management
- MFA support
- Service-to-service authentication

## 10. Development Guidelines
### 10.1 Coding Standards
- Feature-based organization
- Clean architecture principles
- CQRS pattern where applicable
- Comprehensive testing
- Distributed tracing implementation

### 10.2 Testing Strategy
- Unit tests per feature
- Integration tests
- E2E testing with Playwright
- Security testing
- Service integration testing

## 11. Deployment Strategy
### 11.1 Environment Setup
- Development (Local with Aspire)
- Staging
- Production

### 11.2 Infrastructure
- Azure Container Apps (for service deployment)
- SQL Server
- Azure Storage for documents
- Azure KeyVault for secrets
- Application Insights for telemetry

### 11.3 Aspire Configuration
- Service discovery
- Health monitoring
- Distributed tracing
- Resource management
- Configuration management

## 12. Observability
### 12.1 Monitoring
- OpenTelemetry integration
- Aspire dashboard for local development
- Application Insights integration
- Service health monitoring
- Performance metrics

### 12.2 Logging
- Structured logging
- Correlation IDs
- Tenant context logging
- Performance logging

## 13. Future Considerations
1. Scaling individual services
2. Database Sharding
3. Real-time Features
4. Mobile Application
5. Analytics and Reporting
6. Service mesh implementation

## 14. First Sprint Planning
### 14.1 Priority Features
1. Aspire project setup
2. Basic tenant setup
3. User authentication
4. Student registration
5. Document upload
6. Service health monitoring

### 14.2 Technical Tasks
1. Project setup with Aspire integration
2. Identity service implementation
3. Multi-tenant infrastructure
4. Basic API implementation
5. OpenTelemetry setup
6. Local development environment configuration
