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
├── BestMed.RoleService/              # Role management
├── BestMed.PrescriberService/        # Prescriber management
├── BestMed.WarehouseService/         # Warehouse management
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
| `BestMedDbContext` | Abstract base `DbContext` for read-write operations — applies all the above conventions |
| `BestMedReadOnlyDbContext` | Abstract base `DbContext` for read-only operations — disables change tracking and blocks `SaveChanges` |

#### `BestMed.Gateway` — API Gateway

YARP reverse proxy that is the **only externally exposed service**. Routes:

| Route | Upstream Service |
|-------|-----------------|
| `/auth/{**catch-all}` | `authservice` |
| `/users/{**catch-all}` | `userservice` |

Also handles CORS for the Angular frontend.

#### `BestMed.AuthenticateService` — Authentication

Authenticates users against the **BestMed Identity Server** and returns protected tokens for subsequent authorisation. Delegates all credential verification to the Identity Server via `IExternalAuthProvider`, which calls the token endpoint directly.

**Grant type used:** Resource Owner Password Credentials (ROPC) — `client_id: bestmed.homecare.mobile`

**Token security:** All tokens returned to the caller are encrypted with ASP.NET Core **Data Protection** (`IDataProtector`) before leaving the service. Clients must send the protected token back on subsequent calls; the service unprotects it before forwarding to the Identity Server (e.g. on refresh).

**Custom claims** decoded from the Identity Server JWT and available for downstream authorisation:

| Claim | Description |
|-------|-------------|
| `bm_uid` | User's unique GUID |
| `bm_utype` | User type (`Facility`, `Home Care`, `Consumer`, etc.) |
| `bm_name` | Username |
| `bm_firstname` / `bm_lastname` | Display name |
| `bm_role` | Role GUID |

**Password expiry:** The `PasswordExpiredDayLeft` field is included in the login response. Values ≤ 7 should prompt a password-change warning; values ≤ 0 mean the password has expired and must be changed.

| Endpoint | Auth | Rate Limit | Description |
|----------|------|------------|-------------|
| `POST /auth/login` | Anonymous | Heavy | Authenticate via Identity Server (ROPC) and receive a protected token |
| `POST /auth/connect` | Anonymous | Heavy | Refresh an expired token using a protected refresh token |
| `POST /auth/logout` | Required | Standard | Discard token (no server-side revocation — tokens are self-contained JWTs) |

**Configuration keys (`appsettings.json`):**

| Key | Description |
|-----|-------------|
| `IdentityServer:BaseUrl` | Base URL of the BestMed Identity Server |
| `IdentityServer:ClientId` | OAuth client ID (`bestmed.homecare.mobile`) |
| `IdentityServer:Scope` | Requested scope (`api`) |

#### `BestMed.UserService` — User Management

Database-first EF Core service against Azure SQL `[dbo].[User]`. Supports read/write separation via `UserDbContext` (write) and `ReadOnlyUserDbContext` (read).

| Endpoint | Rate Limit | Cache | Description |
|----------|------------|-------|-------------|
| `GET /users/{id}` | Light | `short` (30 s) | Get full user detail by ID |
| `GET /users/external/{externalId}` | Light | `short` (30 s) | Get user by external identity provider ID |
| `GET /users` | Standard | `query` (15 s) | Search/filter with pagination (email, name, type, status, role) |
| `PUT /users/{id}` | Standard | Evicts `users` tag | Update a single user |
| `PUT /users/bulk` | Heavy | Evicts `users` tag | Bulk update multiple users |

#### `BestMed.RoleService` — Role Management

Database-first EF Core service against Azure SQL `[dbo].[UserRole]`. Supports read/write separation via `RoleDbContext` (write) and `ReadOnlyRoleDbContext` (read).

| Endpoint | Rate Limit | Cache | Description |
|----------|------------|-------|-------------|
| `GET /roles/{id}` | Light | `short` (30 s) | Get a role by ID |
| `GET /roles` | Standard | `query` (15 s) | Search/filter roles with pagination (roleCode, roleName, userTypeId) |
| `PUT /roles/{id}` | Standard | Evicts `roles` tag | Update a single role |

#### `BestMed.PrescriberService` — Prescriber Management

Database-first EF Core service against Azure SQL `[dbo].[Prescriber]`. Supports read/write separation via `PrescriberDbContext` (write) and `ReadOnlyPrescriberDbContext` (read).

| Endpoint | Rate Limit | Cache | Description |
|----------|------------|-------|-------------|
| `GET /prescribers/{id}` | Light | `short` (30 s) | Get a prescriber by ID |
| `GET /prescribers` | Standard | `query` (15 s) | Search/filter prescribers with pagination (name, code, AHPRA, email) |
| `PUT /prescribers/{id}` | Standard | Evicts `prescribers` tag | Update a single prescriber |

#### `BestMed.WarehouseService` — Warehouse Management

Database-first EF Core service against Azure SQL `[dbo].[Warehouse]` and related tables. Supports read/write separation via `WarehouseDbContext` (write) and `ReadOnlyWarehouseDbContext` (read). Child entities (`WarehouseBankDetail`, `WarehouseDocument`, `WarehouseHoliday`, `WarehouseRobot`) are loaded eagerly on single-entity GET requests.

| Endpoint | Rate Limit | Cache | Description |
|----------|------------|-------|-------------|
| `GET /warehouses/{id}` | Light | `short` (30 s) | Get a warehouse by ID including bank details, holidays and robots |
| `GET /warehouses` | Standard | `query` (15 s) | Search/filter warehouses with pagination (name, suburb, state, isMultiSite) |
| `PUT /warehouses/{id}` | Standard | Evicts `warehouses` tag | Update a single warehouse; publishes `warehouse-updated` event |

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
   builder.AddMyNewServiceDefaults(); // Service-specific registrations

   var app = builder.Build();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapDefaultEndpoints();
   // app.MapMyEndpoints();

   app.Run();
   ```

4. **Create `ServiceRegistration.cs`** to keep `Program.cs` lean:

   ```csharp
   namespace BestMed.MyNewService;

   public static class ServiceRegistration
   {
       public static IHostApplicationBuilder AddMyNewServiceDefaults(this IHostApplicationBuilder builder)
       {
           // Register DbContexts, HttpClients, and other service-specific dependencies here
           return builder;
       }
   }
   ```

5. **Register in the AppHost** (`BestMed.Platform.AppHost/AppHost.cs`):

   ```csharp
   // Add a project reference to the AppHost .csproj first:
   //   dotnet add BestMed.Platform.AppHost reference BestMed.MyNewService

   var myService = builder.AddProject<Projects.BestMed_MyNewService>("mynewservice")
       .WithHttpHealthCheck("/health");

   // Then add .WithReference(myService) and .WaitFor(myService) to the gateway declaration.
   ```

6. **Add a YARP route** in `BestMed.Gateway/appsettings.json` if the service needs external access:

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

6. **Add `appsettings.json`** with JWT config and EF logging (required by `AddServiceDefaults()`):

   ```json
   {
     "Logging": {
       "LogLevel": {
         "Default": "Information",
         "Microsoft.AspNetCore": "Warning",
         "Microsoft.EntityFrameworkCore": "Warning"
       }
     },
     "AllowedHosts": "*",
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
- **One database per service** — each microservice owns its data store.
- **Azure SQL Database** for all environments (no local SQL containers).
- **SQL authentication** — connection strings use `User Id` and `Password`. Credentials are stored in User Secrets (dev) or Azure Key Vault (deployed environments).
- **Read/write separation** — every service has a read-write context and a read-only context with its own connection string (see [Read/Write Database Separation](#readwrite-database-separation)).
- **Automated provisioning** — `database/Setup-Database.ps1` creates databases, logins, users, and applies schemas.

### Naming Conventions

| Resource | Pattern | Example |
|----------|---------|--------|
| SQL Server | `sqls-bmp-{env}` | `sqls-bmp-dev` |
| Database | `sqldb-bmp-{service}-{env}` | `sqldb-bmp-users-dev` |
| App Login | `bmp-{service}-app` | `bmp-users-app` |

### Service Database Inventory

| Service | Database | Read-Write Context | Read-Only Context | Connection String Keys |
|---------|----------|--------------------|-------------------|------------------------|
| `UserService` | `sqldb-bmp-users-{env}` | `UserDbContext` | `ReadOnlyUserDbContext` | `userdb`, `userdb-readonly` |
| `RoleService` | `sqldb-bmp-roles-{env}` | `RoleDbContext` | `ReadOnlyRoleDbContext` | `roledb`, `roledb-readonly` |
| `PrescriberService` | `sqldb-bmp-prescribers-{env}` | `PrescriberDbContext` | `ReadOnlyPrescriberDbContext` | `prescriberdb`, `prescriberdb-readonly` |
| `WarehouseService` | `sqldb-bmp-warehouses-{env}` | `WarehouseDbContext` | `ReadOnlyWarehouseDbContext` | `warehousedb`, `warehousedb-readonly` |

### Setting Up a New Database

Use the automated provisioning script:

```powershell
cd database

.\Setup-Database.ps1 `
    -ServiceName mynewservice `
    -Environment dev `
    -SqlAdminUser sqladmin `
    -SqlAdminPassword 'YourAdminP@ss!' `
    -AppUser bmp-mynewservice-app `
    -AppPassword 'YourAppP@ss!' `
    -ResourceGroup rg-bmp-dev
```

This will:
1. Create database `sqldb-bmp-mynewservice-dev` on `sqls-bmp-dev`
2. Create SQL login `bmp-mynewservice-app`
3. Grant `db_datareader` and `db_datawriter` roles
4. Run `database/mynewservice/001_InitialSchema.sql` if it exists

Then add the connection string to the service's `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "mynewdb": "Server=sqls-bmp-dev.database.windows.net;Database=sqldb-bmp-mynewservice-dev;User Id=bmp-mynewservice-app;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=False;",
    "mynewdb-readonly": "Server=sqls-bmp-dev.database.windows.net;Database=sqldb-bmp-mynewservice-dev;User Id=bmp-mynewservice-app;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

> **Note:** Both keys can point to the same server until a read replica is provisioned.

4. **Register with Aspire** in `Program.cs`:

   ```csharp
   builder.AddSqlServerDbContext<MyNewDbContext>("mynewdb", configureSettings: settings =>
   {
       settings.DisableHealthChecks = true;
   });
   ```

### Read/Write Database Separation

The platform supports separating database access into **read-only** and **read-write** connections (e.g., to route reads to a replica). This is implemented via two base DbContext classes in `BestMed.Data`:

- **`BestMedDbContext`** — for write operations (insert, update, delete). Includes audit stamping and soft-delete interception.
- **`BestMedReadOnlyDbContext`** — for read operations only. Disables change tracking and throws if `SaveChanges` is called.

#### Applying to a Service

1. **Create a read-only DbContext** inheriting from `BestMedReadOnlyDbContext`:

   ```csharp
   public class ReadOnlyMyDbContext : BestMedReadOnlyDbContext
   {
       public ReadOnlyMyDbContext(DbContextOptions<ReadOnlyMyDbContext> options) : base(options) { }

       public DbSet<MyEntity> MyEntities { get; set; } = null!;

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
           modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
       }
   }
   ```

2. **Register both contexts** in `Program.cs` with separate Aspire connection names:

   ```csharp
   // Read-write context
   builder.AddSqlServerDbContext<MyDbContext>("mydb", configureSettings: settings =>
   {
       settings.DisableHealthChecks = true;
   });

   // Read-only context (separate connection string for read replica)
   builder.AddSqlServerDbContext<ReadOnlyMyDbContext>("mydb-readonly", configureSettings: settings =>
   {
       settings.DisableHealthChecks = true;
   });
   ```

3. **Inject the appropriate context** in your endpoints:
   - Read endpoints (GET) → inject the read-only context
   - Write endpoints (POST/PUT/DELETE) → inject the read-write context

4. **Add the connection string** for the read replica in `appsettings.{Environment}.json`:

   ```json
   {
     "ConnectionStrings": {
       "mydb": "Server=sqls-bmp-dev.database.windows.net;Database=sqldb-bmp-myservice-dev;User Id=bmp-myservice-app;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=False;",
       "mydb-readonly": "Server=sqls-bmp-dev-replica.database.windows.net;Database=sqldb-bmp-myservice-dev;User Id=bmp-myservice-app;Password=CHANGE_ME;Encrypt=True;TrustServerCertificate=False;"
     }
   }
   ```

   > **Tip:** If no read replica is available, you can point both connection strings to the same server.

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
| `ConnectionStrings__userdb` | *(from Key Vault)* | UserService only — SQL auth connection string |
| `IdentityServer__BaseUrl` | *(Identity Server URL)* | AuthenticateService only |
| `IdentityServer__ClientId` | `bestmed.homecare.mobile` | AuthenticateService only |
| `IdentityServer__Scope` | `api` | AuthenticateService only |

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


---

## VI. Inter-Service Communication

The platform uses **two complementary patterns** for services to talk to each other. The rule for choosing is simple:

```
Need data NOW to complete a response?
  └─ YES → Direct HTTP  (typed client + caching decorator)
  └─ NO  → Did something happen that others should react to?
              └─ YES → Azure Service Bus (publish an integration event)
```

Never use HTTP for fire-and-forget notifications, and never use Service Bus when a caller needs to block on a result.

### Decision Matrix

| Scenario | Pattern | Reason |
|----------|---------|--------|
| Service A needs data from Service B to build its response | **HTTP** | Synchronous — the caller must block until it has the data |
| Service A changes something and other services should react | **Service Bus** | Async fan-out — no single consumer; each subscriber reacts independently |
| Cache invalidation after a write in another service | **Service Bus** | The source service should not know or care who is caching its data |
| Auditing, notifications, or workflow triggers | **Service Bus** | Loose coupling — new consumers can be added without changing the publisher |

### Current Event Inventory

| Topic | Publisher | Subscription | Subscriber | Purpose |
|-------|-----------|--------------|------------|---------|
| `role-updated` | RoleService | `userservice-role-updated` | UserService | Invalidate the in-memory role cache |
| `prescriber-updated` | PrescriberService | `userservice-prescriber-updated` | UserService | Invalidate the in-memory prescriber cache |
| `user-status-changed` | UserService | *(none yet)* | *(future consumers)* | Notify downstream services when a user is activated/deactivated |
| `warehouse-updated` | WarehouseService | *(none yet)* | *(future consumers)* | Notify downstream services when warehouse data changes |

### Shared Contracts

Cross-service DTOs live in `BestMed.Common/Contracts/` — **never reference another service's internal DTOs directly**. This avoids circular project dependencies and keeps each service independently deployable.

| Contract | Used By | Matches |
|----------|---------|---------|
| `RoleContract` | UserService HTTP client | RoleService `/roles/{id}` response shape |
| `PrescriberContract` | UserService HTTP client | PrescriberService `/prescribers/{id}` response shape |

When adding a new cross-service HTTP client, add its response contract here first.

---

### Pattern 1 — Direct HTTP (Synchronous Queries)

Used when a service needs live data from another service **within the same request/response cycle**.

Every HTTP client follows this layered structure:

```
Endpoint
  └─ IMyServiceClient              (interface — injected into endpoint)
       └─ CachingMyServiceClient   (decorator — IMemoryCache, 5 min TTL)
            └─ MyServiceClient     (HttpClient — Aspire service discovery)
```

#### Step-by-step: adding an HTTP client to a new consuming service

**Step 1** — Add a shared contract to `BestMed.Common/Contracts/` if one does not exist yet:

```csharp
// BestMed.Common/Contracts/UserContract.cs
namespace BestMed.Common.Contracts;

public sealed record UserContract
{
    public Guid Id { get; init; }
    public string? Email { get; init; }
    public bool IsActive { get; init; }
}
```

**Step 2** — Create the interface and HTTP client inside the consuming service:

```csharp
// BestMed.MyService/Clients/IUserServiceClient.cs
using BestMed.Common.Contracts;

public interface IUserServiceClient
{
    Task<UserContract?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
}

// BestMed.MyService/Clients/UserServiceClient.cs
internal sealed class UserServiceClient(HttpClient httpClient) : IUserServiceClient
{
    public async Task<UserContract?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<UserContract>($"/users/{userId}", ct);
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
```

**Step 3** — Add a caching decorator:

```csharp
// BestMed.MyService/Clients/CachingUserServiceClient.cs
internal sealed class CachingUserServiceClient(IUserServiceClient inner, IMemoryCache cache)
    : IUserServiceClient
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(5);

    public async Task<UserContract?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
        => await cache.GetOrCreateAsync($"users:{userId}", async e =>
        {
            e.AbsoluteExpirationRelativeToNow = Ttl;
            return await inner.GetUserByIdAsync(userId, ct);
        });

    // Call this from an IEventHandler when a UserStatusChangedEvent is received.
    public static void InvalidateById(IMemoryCache cache, Guid userId)
        => cache.Remove($"users:{userId}");
}
```

**Step 4** — Register in the consuming service's `ServiceRegistration.cs`:

```csharp
builder.Services.AddMemoryCache(); // skip if already registered

builder.Services.AddHttpClient<UserServiceClient>(c =>
    c.BaseAddress = new Uri("https+http://userservice"));

builder.Services.AddSingleton<IUserServiceClient>(sp =>
    new CachingUserServiceClient(
        sp.GetRequiredService<UserServiceClient>(),
        sp.GetRequiredService<IMemoryCache>()));
```

**Step 5** — Wire the service reference in `AppHost.cs`:

```csharp
var myService = builder.AddProject<Projects.BestMed_MyService>("myservice")
    .WithReference(userService)   // enables https+http://userservice resolution
    .WaitFor(userService);
```

---

### Pattern 2 — Azure Service Bus (Async Events)

Used when a service writes data and needs to **notify other services** without knowing who they are or waiting for a response.

All infrastructure is already in place:
- `BestMed.Common/Messaging/` — `IIntegrationEvent`, `IntegrationEvent`, `IEventPublisher`, `IEventHandler<T>`
- `BestMed.Common/Messaging/Events/` — shared event records
- `BestMed.Platform.ServiceDefaults/Messaging/` — `ServiceBusEventPublisher`, `ServiceBusSubscriberWorker`, `MessagingExtensions`

#### Publishing an event

**Step 1** — Define the event in `BestMed.Common/Messaging/Events/`:

```csharp
// BestMed.Common/Messaging/Events/MyThingChangedEvent.cs
namespace BestMed.Common.Messaging.Events;

public sealed record MyThingChangedEvent : IntegrationEvent
{
    public required Guid ThingId { get; init; }
    public required string? NewValue { get; init; }
}
```

> **Naming convention:** `{Subject}{PastTenseVerb}Event` → topic = kebab-case without the `Event` suffix.
> `MyThingChangedEvent` → topic `my-thing-changed`. Handled automatically by `MessagingExtensions.ToTopicName()`.

**Step 2** — Register the publisher in the service's `ServiceRegistration.cs`:

```csharp
builder.AddServiceBusPublisher(); // registers IEventPublisher backed by Azure Service Bus
```

**Step 3** — Inject `IEventPublisher` into the endpoint and publish **after** the write commits:

```csharp
private static async Task<IResult> UpdateAsync(
    Guid id,
    UpdateMyThingRequest request,
    MyDbContext db,
    IEventPublisher eventPublisher,
    CancellationToken ct)
{
    var thing = await db.Things.FindAsync([id], ct);
    if (thing is null) return Results.NotFound();

    thing.Value = request.Value;
    await db.SaveChangesAsync(ct);   // commit first

    await eventPublisher.PublishAsync(new MyThingChangedEvent
    {
        ThingId = thing.Id,
        NewValue = thing.Value
    }, ct);

    return Results.Ok(thing.ToDto());
}
```

**Step 4** — Add the topic to `AppHost.cs`:

```csharp
var serviceBus = builder.AddAzureServiceBus("servicebus")
    .AddTopic("role-updated",        ["userservice-role-updated"])
    .AddTopic("prescriber-updated",  ["userservice-prescriber-updated"])
    .AddTopic("user-status-changed", [])
    .AddTopic("my-thing-changed",    []);  // ← add your topic; fill subscriptions as consumers are created
```

**Step 5** — Add `.WithReference(serviceBus)` to the publishing service in `AppHost.cs`:

```csharp
var myService = builder.AddProject<Projects.BestMed_MyService>("myservice")
    .WithReference(serviceBus)
    .WaitFor(serviceBus);
```

#### Subscribing to an event

**Step 1** — Add the Aspire Service Bus package to the consuming service if not already present:

```powershell
dotnet add BestMed.MyService package Aspire.Azure.Messaging.ServiceBus --version 9.3.1
```

**Step 2** — Implement `IEventHandler<TEvent>` inside the consuming service:

```csharp
// BestMed.MyService/Messaging/MyThingChangedEventHandler.cs
using BestMed.Common.Messaging;
using BestMed.Common.Messaging.Events;

namespace BestMed.MyService.Messaging;

internal sealed class MyThingChangedEventHandler(ILogger<MyThingChangedEventHandler> logger)
    : IEventHandler<MyThingChangedEvent>
{
    public Task HandleAsync(MyThingChangedEvent @event, CancellationToken ct)
    {
        // e.g. invalidate a cache, trigger a workflow, write an audit record
        logger.LogInformation("Thing {ThingId} changed to {Value}", @event.ThingId, @event.NewValue);
        return Task.CompletedTask;
    }
}
```

**Step 3** — Register in the consuming service's `ServiceRegistration.cs`:

```csharp
builder.AddServiceBusSubscriber<MyThingChangedEvent, MyThingChangedEventHandler>(
    subscriptionName: "myservice-my-thing-changed");
```

**Step 4** — Add the subscription name to the topic in `AppHost.cs`:

```csharp
.AddTopic("my-thing-changed", ["myservice-my-thing-changed"])
//                              ↑ add your subscription here
```

**Step 5** — Add `.WithReference(serviceBus)` to the consuming service in `AppHost.cs`:

```csharp
var myService = builder.AddProject<Projects.BestMed_MyService>("myservice")
    .WithReference(serviceBus)
    .WaitFor(serviceBus);
```

`ServiceBusSubscriberWorker` starts automatically as a hosted background service. Messages are completed on success, abandoned on handler exception (allowing retry), and dead-lettered after repeated failures.

### Subscription Naming Convention

```
{consuming-service-kebab-name}-{topic-name}

Examples:
  user-service-role-updated
  user-service-prescriber-updated
  auth-service-user-status-changed
```

This makes it immediately clear in the Azure portal which service owns which subscription, without needing to look at code.
