# BestMed Platform

A .NET 10 microservices solution built on [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) with an Angular frontend. The platform uses a gateway-first architecture where all external traffic flows through a single YARP reverse-proxy, backend services communicate via Aspire service discovery, and every request (except explicit opt-outs) requires a valid JWT issued by the centralised authentication service.

---

## I. Solution Overview

### Architecture

```
┌──────────────┐       ┌─────────────┐
│   Angular    │──────▶│   Gateway   │  (YARP reverse proxy, external entry point)
│   Frontend   │       │  /auth/*    │──▶ AuthenticateService
└──────────────┘       │  /users/*   │──▶ UserService
                       └─────────────┘
                              │
            ┌─────────────────┴─────────────────┐
            ▼                                   ▼
   AuthenticateService                    UserService
   (JWT issuance)                         (user CRUD)
                                              │
                                        Azure SQL DB
```

All inter-service HTTP calls use **Aspire service discovery** (`https+http://servicename`) and are restricted to **HTTPS only**.

### Standards Applied Across All Services

Every service that calls `builder.AddServiceDefaults()` automatically receives:

| Concern | Implementation | Details |
|---------|---------------|---------|
| **Authentication** | JWT Bearer (centralised) | Tokens validated against shared `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` config. 30 s clock skew. |
| **Authorisation** | Fallback policy | All endpoints require authentication by default; opt out with `.AllowAnonymous()`. |
| **Rate Limiting** | Three fixed-window tiers | `light` (60 req/10 s), `standard` (20 req/10 s), `heavy` (5 req/30 s) — partitioned by client IP. |
| **Output Caching** | In-memory (Redis-ready) | `"short"` policy (30 s) for entity reads, `"query"` policy (15 s, varies by query string) for searches. |
| **Security Headers** | Middleware | `X-Content-Type-Options`, `X-Frame-Options: DENY`, `Referrer-Policy`, `Permissions-Policy`. |
| **Resilience** | Standard resilience handler | Retries, circuit breakers, and timeouts on all outgoing `HttpClient` calls. |
| **Observability** | OpenTelemetry | Metrics (ASP.NET Core, HTTP, runtime), distributed tracing, structured logging via OTLP exporter. |
| **Health Checks** | `/health` and `/alive` | Aspire health probes; health endpoints are anonymous and excluded from tracing. |
| **Service Discovery** | HTTPS-only | Inter-service calls resolved via Aspire; plaintext schemes are disallowed. |

### Libraries & Key Packages

| Package | Used In | Purpose |
|---------|---------|---------|
| `Aspire.AppHost.Sdk` | AppHost | Orchestrates all services and frontend |
| `Aspire.Microsoft.EntityFrameworkCore.SqlServer` | UserService | Aspire-managed EF Core + SQL Server integration |
| `Yarp.ReverseProxy` | Gateway | Reverse proxy / API gateway |
| `Microsoft.AspNetCore.Authentication.JwtBearer` | ServiceDefaults | Centralised JWT validation |
| `Microsoft.Extensions.Http.Resilience` | ServiceDefaults | Retry, circuit breaker, timeout policies |
| `OpenTelemetry.*` | ServiceDefaults | Metrics, tracing, logging |

---

## II. Project Overview

### Solution Structure

```
BestMed.Platform/
├── BestMed.Platform.AppHost/          # Aspire orchestrator
├── BestMed.Platform.ServiceDefaults/  # Shared cross-cutting concerns
├── BestMed.Common/                   # Shared constants, models, helpers
├── BestMed.Data/                     # Shared EF Core base classes & conventions
├── BestMed.Gateway/                  # YARP reverse proxy (external entry point)
├── BestMed.AuthenticateService/      # Authentication & token issuance
├── BestMed.UserService/              # User management CRUD
└── BestMed.Platform.Tests/            # Integration tests
```

### Project Details

#### `BestMed.Platform.AppHost` — Aspire Orchestrator

The entry point that wires up all services, defines startup dependencies (`WaitFor`), health probes, and service references. Manages the Angular frontend via `AddNpmApp`.

**Key file:** `AppHost.cs`

#### `BestMed.Platform.ServiceDefaults` — Cross-Cutting Concerns

A shared project (`IsAspireSharedProject`) referenced by every service. Calling `builder.AddServiceDefaults()` registers authentication, authorisation, rate limiting, output caching, OpenTelemetry, health checks, resilience, and service discovery. Calling `app.MapDefaultEndpoints()` adds security headers, rate-limiter middleware, output-cache middleware, and health-check endpoints.

**Key file:** `Extensions.cs`

#### `BestMed.Common` — Shared Library (Pure .NET)

No ASP.NET or EF Core dependencies. Contains types reusable across all services:

| File | Purpose |
|------|---------|
| `Models/PagedResponse<T>` | Standard pagination envelope |
| `Models/BulkOperationResult` | Standard result for bulk mutations |
| `Constants/PaginationDefaults` | `DefaultPage`, `DefaultPageSize`, `MaxPageSize`, `ClampPageSize()` |
| `Constants/SortDirection` | `Ascending` / `Descending` constants and `IsAscending()` helper |
| `Helpers/Guard` | `NotNull()`, `NotNullOrWhiteSpace()`, `NotEmpty()` argument guards |

#### `BestMed.Data` — Shared EF Core Conventions

Contains base classes and interfaces that enforce database conventions automatically:

| File | Purpose |
|------|---------|
| `Entities/IEntity` | Marker interface — all entities have a `Guid Id` |
| `Entities/IAuditable` | `CreatedAt` / `UpdatedAt` — auto-stamped on `SaveChanges` |
| `Entities/ISoftDeletable` | `IsDeleted` / `DeletedAt` — global query filter applied automatically; `Remove()` intercepted as a soft-delete |
| `BestMedDbContext` | Abstract base `DbContext` that applies all the above conventions |

#### `BestMed.Gateway` — API Gateway

YARP reverse proxy that is the **only externally exposed service**. Routes:

| Route | Upstream Service |
|-------|-----------------|
| `/auth/{**catch-all}` | `authservice` |
| `/users/{**catch-all}` | `userservice` |

Also handles CORS for the Angular frontend.

#### `BestMed.AuthenticateService` — Authentication

Issues and validates JWTs. Delegates credential verification to an external auth provider via `IExternalAuthProvider`. Endpoints:

| Endpoint | Auth | Rate Limit | Description |
|----------|------|------------|-------------|
| `POST /auth/login` | Anonymous | Heavy | Authenticate and receive a JWT |
| `POST /auth/connect` | Anonymous | Heavy | Refresh an expired token |
| `POST /auth/logout` | Required | Standard | Revoke the current token |

#### `BestMed.UserService` — User Management

Database-first EF Core service against Azure SQL. Entities implement `IEntity` and `IAuditable` for automatic conventions.

| Endpoint | Rate Limit | Cache | Description |
|----------|------------|-------|-------------|
| `GET /users/{id}` | Light | `short` (30 s) | Get user by ID with addresses |
| `GET /users/external/{externalId}` | Light | `short` (30 s) | Get user by external identity ID |
| `GET /users` | Standard | `query` (15 s) | Search/filter with pagination |
| `PUT /users/{id}` | Standard | Evicts `users` tag | Update a single user |
| `PUT /users/bulk` | Heavy | Evicts `users` tag | Bulk update multiple users |

#### `BestMed.Platform.Tests` — Integration Tests

Aspire-based integration tests that spin up the full distributed application.

---

## III. Development Guidelines

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/) (for Angular frontend)
- [Visual Studio 2026 17.x+](https://visualstudio.microsoft.com/) or VS Code with C# Dev Kit
- Azure CLI (for Azure SQL authentication: `az login`)

### Creating a New Service

1. **Create the project:**

   ```powershell
   dotnet new web -n BestMed.MyNewService -f net10.0
   dotnet sln add BestMed.MyNewService\BestMed.MyNewService.csproj
   ```

2. **Add project references:**

   ```powershell
   dotnet add BestMed.MyNewService reference `
       BestMed.Platform.ServiceDefaults\BestMed.Platform.ServiceDefaults.csproj `
       BestMed.Common\BestMed.Common.csproj `
       BestMed.Data\BestMed.Data.csproj
   ```

3. **Write `Program.cs`** following the standard pattern:

   ```csharp
   var builder = WebApplication.CreateBuilder(args);

   builder.AddServiceDefaults();
   // Add service-specific registrations here

   var app = builder.Build();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapDefaultEndpoints();
   // app.MapMyEndpoints();

   app.Run();
   ```

4. **Register in the AppHost** (`BestMed.Platform.AppHost/AppHost.cs`):

   ```csharp
   // Add a project reference to the AppHost .csproj first:
   //   dotnet add BestMed.Platform.AppHost reference BestMed.MyNewService

   var myService = builder.AddProject<Projects.BestMed_MyNewService>("mynewservice")
       .WithHttpHealthCheck("/health");
   ```

5. **Add a YARP route** in `BestMed.Gateway/appsettings.json` if the service needs external access:

   ```json
   "mynew-route": {
     "ClusterId": "mynewservice",
     "Match": { "Path": "/mynew/{**catch-all}" }
   }
   ```

   And a cluster:

   ```json
   "mynewservice": {
     "Destinations": {
       "destination1": { "Address": "https+http://mynewservice" }
     }
   }
   ```

6. **Add `appsettings.json`** with JWT config (required by ServiceDefaults):

   ```json
   {
     "Jwt": {
       "Issuer": "BestMed",
       "Audience": "BestMed.Services",
       "Key": "CHANGE-THIS-TO-A-SECURE-KEY-AT-LEAST-32-CHARS!"
     }
   }
   ```

7. **Define endpoints** using Minimal APIs. All endpoints are authenticated by default:

   ```csharp
   public static class MyEndpoints
   {
       public static IEndpointRouteBuilder MapMyEndpoints(this IEndpointRouteBuilder routes)
       {
           var group = routes.MapGroup("/mynew")
               .WithTags("MyNew");

           group.MapGet("/{id:guid}", GetByIdAsync)
               .RequireRateLimiting(Extensions.RateLimitLight)
               .CacheOutput("short");

           group.MapPost("/", CreateAsync)
               .RequireRateLimiting(Extensions.RateLimitStandard);

           return routes;
       }
   }
   ```

### Building & Running

**Run the full solution (recommended):**

```powershell
# From the solution root
dotnet run --project BestMed.Platform.AppHost
```

This starts the Aspire dashboard, all services, and the Angular frontend with service discovery.

**Run with a specific environment:**

```powershell
# Development (default)
dotnet run --project BestMed.Platform.AppHost

# UAT
dotnet run --project BestMed.Platform.AppHost --launch-profile uat

# Or set the environment variable directly
$env:ASPNETCORE_ENVIRONMENT = "UAT"
dotnet run --project BestMed.Platform.AppHost
```

**Run a single service for isolated debugging:**

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project BestMed.UserService
```

> **Note:** Running a single service outside Aspire means service discovery is unavailable. You will need to provide connection strings and configuration manually.

**Build only (no run):**

```powershell
dotnet build                          # Debug
dotnet build -c Release               # Release
```

### Configuration Layering

Configuration loads in order (last wins):

1. `appsettings.json` — shared defaults (JWT keys, logging)
2. `appsettings.{Environment}.json` — environment overrides (connection strings, feature flags)
3. Environment variables — CI/CD and container overrides
4. User Secrets (`secrets.json`) — local developer credentials (Development only)

Supported environments: `Development`, `UAT`, `Production`.

---

## IV. Database Guidelines

### Architecture Decisions

- **Database-first** approach — the database schema is the source of truth.
- **One database per service** — each microservice owns its data store (e.g., `BestMedUsers` for `UserService`).
- **Azure SQL Database** for all environments (no local SQL containers).
- **Azure AD authentication** — connection strings use `Authentication=Active Directory Default`.

### Setting Up a New Database

1. **Create the Azure SQL Database** (via portal, CLI, or IaC):

   ```powershell
   az sql db create `
       --resource-group bestmed-rg `
       --server bestmed-dev `
       --name BestMedMyNewDb `
       --service-objective S0
   ```

2. **Grant the service identity access:**

   ```sql
   CREATE USER [bestmed-mynewservice] FROM EXTERNAL PROVIDER;
   ALTER ROLE db_datareader ADD MEMBER [bestmed-mynewservice];
   ALTER ROLE db_datawriter ADD MEMBER [bestmed-mynewservice];
   ```

3. **Add the connection string** to the service's `appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "mynewdb": "Server=bestmed-dev.database.windows.net;Database=BestMedMyNewDb;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;"
     }
   }
   ```

4. **Register with Aspire** in `Program.cs`:

   ```csharp
   builder.AddSqlServerDbContext<MyNewDbContext>("mynewdb", configureSettings: settings =>
   {
       settings.DisableHealthChecks = true;
   });
   ```

### Scaffolding Entities (Database-First)

When the database schema changes, re-scaffold:

```powershell
cd BestMed.MyNewService

dotnet ef dbcontext scaffold `
    "Name=ConnectionStrings:mynewdb" `
    Microsoft.EntityFrameworkCore.SqlServer `
    --output-dir Entities `
    --context-dir Data `
    --context MyNewDbContext `
    --force `
    --no-onconfiguring
```

After scaffolding:

1. **Have the DbContext inherit from `BestMedDbContext`** instead of `DbContext`:

   ```csharp
   public partial class MyNewDbContext : BestMedDbContext
   {
       public MyNewDbContext(DbContextOptions<MyNewDbContext> options) : base(options) { }

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);  // ← applies soft-delete filters
           modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyNewDbContext).Assembly);
           OnModelCreatingPartial(modelBuilder);
       }

       partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
   }
   ```

2. **Implement interfaces on entities** as appropriate:

   ```csharp
   // Every entity should implement IEntity
   public partial class MyEntity : IEntity, IAuditable, ISoftDeletable
   ```

   - `IEntity` — `Guid Id` primary key
   - `IAuditable` — `CreatedAt` auto-set on insert, `UpdatedAt` auto-set on update (by `BestMedDbContext`)
   - `ISoftDeletable` — `IsDeleted` / `DeletedAt` auto-managed; global query filter hides deleted rows

3. **Do not manually set `CreatedAt` / `UpdatedAt`** — `BestMedDbContext.SaveChanges` handles this.

4. **Do not call `Remove()` on soft-deletable entities** expecting a hard delete — it will be intercepted as a soft-delete. If you genuinely need a hard delete, set the entry state directly.

### Schema Migrations

Since the project uses database-first, schema changes are made directly in the database:

1. Apply the DDL change to the target database (via migration scripts, SSMS, or your IaC tool).
2. Re-scaffold the entities (see above).
3. Verify the `ISoftDeletable` / `IAuditable` interfaces are re-applied on the scaffolded entities (scaffolding overwrites entity files).
4. Commit the updated entity classes.

---

## V. Deployment Guidelines (CI/CD via Azure DevOps)

### Environment Strategy

| Environment | Branch | Azure SQL Server | Purpose |
|-------------|--------|-----------------|---------|
| Development | `develop` | `bestmed-dev.database.windows.net` | Local debugging + shared dev |
| UAT | `release/*` | `bestmed-uat.database.windows.net` | Stakeholder testing |
| Production | `main` | `bestmed-prod.database.windows.net` | Live |

### Pipeline Overview

```
┌──────────┐     ┌──────────┐     ┌──────────┐     ┌──────────┐
│  Build   │────▶│  Test    │────▶│ Publish  │────▶│  Deploy  │
│  (.NET)  │     │  (xUnit) │     │ (Docker) │     │ (Azure)  │
└──────────┘     └──────────┘     └──────────┘     └──────────┘
```

### Sample `azure-pipelines.yml`

```yaml
trigger:
  branches:
    include:
      - main
      - develop
      - release/*

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'
  dotnetVersion: '10.0.x'
  containerRegistry: 'bestmedacr.azurecr.io'

stages:
  # ──────────────────────────────────────────────
  # Stage 1: Build & Test
  # ──────────────────────────────────────────────
  - stage: Build
    displayName: 'Build & Test'
    jobs:
      - job: BuildAndTest
        displayName: 'Build, Test, Publish'
        steps:
          - task: UseDotNet@2
            displayName: 'Install .NET SDK'
            inputs:
              packageType: sdk
              version: $(dotnetVersion)

          - script: dotnet restore
            displayName: 'Restore NuGet packages'

          - script: dotnet build --configuration $(buildConfiguration) --no-restore
            displayName: 'Build solution'

          - script: dotnet test --configuration $(buildConfiguration) --no-build --logger trx --results-directory $(Build.ArtifactStagingDirectory)/TestResults
            displayName: 'Run tests'

          - task: PublishTestResults@2
            displayName: 'Publish test results'
            inputs:
              testResultsFormat: VSTest
              testResultsFiles: '$(Build.ArtifactStagingDirectory)/TestResults/*.trx'
            condition: always()

          # Publish each service as a container image
          - script: |
              dotnet publish BestMed.Gateway -c $(buildConfiguration) -o $(Build.ArtifactStagingDirectory)/gateway
              dotnet publish BestMed.AuthenticateService -c $(buildConfiguration) -o $(Build.ArtifactStagingDirectory)/authservice
              dotnet publish BestMed.UserService -c $(buildConfiguration) -o $(Build.ArtifactStagingDirectory)/userservice
            displayName: 'Publish services'

          - task: PublishBuildArtifacts@1
            displayName: 'Publish artifacts'
            inputs:
              PathtoPublish: $(Build.ArtifactStagingDirectory)
              ArtifactName: 'drop'

  # ──────────────────────────────────────────────
  # Stage 2: Deploy to UAT
  # ──────────────────────────────────────────────
  - stage: DeployUAT
    displayName: 'Deploy to UAT'
    dependsOn: Build
    condition: and(succeeded(), startsWith(variables['Build.SourceBranch'], 'refs/heads/release/'))
    jobs:
      - deployment: DeployUAT
        displayName: 'Deploy to UAT'
        environment: 'uat'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: AzureWebApp@1
                  displayName: 'Deploy Gateway'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-gateway-uat'
                    package: '$(Pipeline.Workspace)/drop/gateway'

                - task: AzureWebApp@1
                  displayName: 'Deploy AuthService'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-authservice-uat'
                    package: '$(Pipeline.Workspace)/drop/authservice'

                - task: AzureWebApp@1
                  displayName: 'Deploy UserService'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-userservice-uat'
                    package: '$(Pipeline.Workspace)/drop/userservice'

  # ──────────────────────────────────────────────
  # Stage 3: Deploy to Production
  # ──────────────────────────────────────────────
  - stage: DeployProd
    displayName: 'Deploy to Production'
    dependsOn: Build
    condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
    jobs:
      - deployment: DeployProd
        displayName: 'Deploy to Production'
        environment: 'production'
        strategy:
          runOnce:
            deploy:
              steps:
                - task: AzureWebApp@1
                  displayName: 'Deploy Gateway'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-gateway-prod'
                    package: '$(Pipeline.Workspace)/drop/gateway'

                - task: AzureWebApp@1
                  displayName: 'Deploy AuthService'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-authservice-prod'
                    package: '$(Pipeline.Workspace)/drop/authservice'

                - task: AzureWebApp@1
                  displayName: 'Deploy UserService'
                  inputs:
                    azureSubscription: 'BestMed-Azure-Connection'
                    appName: 'bestmed-userservice-prod'
                    package: '$(Pipeline.Workspace)/drop/userservice'
```

### Azure App Service Configuration

For each deployed service, set these application settings:

| Setting | Value | Notes |
|---------|-------|-------|
| `ASPNETCORE_ENVIRONMENT` | `UAT` or `Production` | Determines which `appsettings.{env}.json` loads |
| `Jwt__Issuer` | `BestMed` | Double underscore for nested config |
| `Jwt__Audience` | `BestMed.Services` | |
| `Jwt__Key` | *(from Key Vault)* | Use Key Vault reference: `@Microsoft.KeyVault(...)` |
| `ConnectionStrings__userdb` | *(from Key Vault)* | UserService only |
| `ExternalAuth__BaseUrl` | *(provider URL)* | AuthenticateService only |

### Security Recommendations for Production

- Store `Jwt:Key` and connection strings in **Azure Key Vault** and reference them via App Service Key Vault references.
- Enable **Managed Identity** on each App Service and grant it access to its database (eliminates stored credentials).
- The Gateway should be the only service with a **public endpoint**. All backend services should use private endpoints or VNet integration.
- Enable **HTTPS Only** on all App Services.
- Configure **deployment slots** for zero-downtime deployments on the Gateway and critical services.

### Enabling Redis for Production Caching

When scaling to multiple instances, switch from in-memory output cache to Redis:

1. Add `Aspire.Hosting.Redis` to the AppHost project.
2. Uncomment `var redis = builder.AddRedis("redis");` in `AppHost.cs`.
3. Add `.WithReference(redis)` to each service that needs distributed caching.
4. Add `Aspire.StackExchange.Redis.OutputCaching` to each consuming service.
5. Call `builder.AddRedisOutputCache("redis");` after `AddServiceDefaults()` in each service's `Program.cs`.

No endpoint code changes are needed — cache policies, tags, and eviction calls remain identical.
