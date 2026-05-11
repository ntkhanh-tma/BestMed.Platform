# Database Scripts

This folder contains the SQL scripts for the BestMed Platform databases.
Each service has its own subfolder with numbered scripts executed in order.

## Folder Structure

```
database/
├── Setup-Database.ps1              # Automated provisioning script
├── README.md
├── users/
│   └── 001_InitialSchema.sql       # UserService schema
├── roles/
│   └── 001_InitialSchema.sql       # RoleService schema
├── prescribers/
│   └── 001_InitialSchema.sql       # PrescriberService schema
└── warehouses/
    └── 001_InitialSchema.sql       # WarehouseService schema
```

## Naming Conventions

| Resource | Pattern | Example |
|----------|---------|---------|
| SQL Server | `sqls-bmp-{env}` | `sqls-bmp-dev` |
| Database | `sqldb-bmp-{service}-{env}` | `sqldb-bmp-users-dev` |
| App Login | `bmp-{service}-app` | `bmp-users-app` |

## Service Database Inventory

| Service | Script Folder | Database | Connection Keys | App Login |
|---------|--------------|----------|-----------------|-----------|
| UserService | `users/` | `sqldb-bmp-users-{env}` | `userdb`, `userdb-readonly` | `bmp-users-app` |
| RoleService | `roles/` | `sqldb-bmp-roles-{env}` | `roledb`, `roledb-readonly` | `bmp-roles-app` |
| PrescriberService | `prescribers/` | `sqldb-bmp-prescribers-{env}` | `prescriberdb`, `prescriberdb-readonly` | `bmp-prescribers-app` |
| WarehouseService | `warehouses/` | `sqldb-bmp-warehouses-{env}` | `warehousedb`, `warehousedb-readonly` | `bmp-warehouses-app` |

## Authentication

All connections use **SQL authentication** (`User Id` / `Password`).
Passwords should be stored securely:
- **Development**: .NET User Secrets (`dotnet user-secrets set "ConnectionStrings:userdb" "..."`)
- **UAT/Production**: Azure Key Vault references in App Service configuration

## Automated Provisioning

Use `Setup-Database.ps1` to create a database, SQL login, app user, and apply the initial schema in one command.

### Prerequisites

- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) (`az login` completed)
- [sqlcmd](https://learn.microsoft.com/sql/tools/sqlcmd/sqlcmd-utility) available on PATH

### Provision a single service database

```powershell
cd database

.\Setup-Database.ps1 `
    -ServiceName users `
    -Environment dev `
    -SqlAdminUser sqladmin `
    -SqlAdminPassword 'YourAdminP@ss!' `
    -AppUser bmp-users-app `
    -AppPassword 'YourAppP@ss!' `
    -ResourceGroup rg-bmp-dev
```

### Provision all service databases at once

```powershell
cd database

@("users","roles","prescribers","warehouses") | ForEach-Object {
    .\Setup-Database.ps1 `
        -ServiceName $_ `
        -Environment dev `
        -SqlAdminUser sqladmin `
        -SqlAdminPassword 'YourAdminP@ss!' `
        -AppUser "bmp-$_-app" `
        -AppPassword 'YourAppP@ss!' `
        -ResourceGroup rg-bmp-dev
}
```

### Adding a new service database

1. Create a new folder under `database/` matching the service name (e.g. `database/newservice/`)
2. Add `001_InitialSchema.sql` with the table definitions
3. Run `Setup-Database.ps1 -ServiceName newservice ...`
4. Add connection strings to the service's `appsettings.Development.json`

## Database-First Workflow

1. **Modify schema** — Create a new numbered SQL script (e.g., `002_AddColumnX.sql`) in the service's folder
2. **Apply to database** — Run the script against the target Azure SQL database
3. **Re-scaffold entities** — From the service project directory:

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:userdb" Microsoft.EntityFrameworkCore.SqlServer `
    --output-dir Entities `
    --context-dir Data `
    --context UserDbContext `
    --force `
    --no-onconfiguring
```

4. **Update DTOs and mappings** — If new columns were added, update the DTOs and mapping extensions

## Environments

| Environment | Server | Auth |
|---|---|---|
| Dev | sqls-bmp-dev.database.windows.net | SQL Auth |
| BAT | sqls-bmp-bat.database.windows.net | SQL Auth |
| Prodsup | sqls-bmp-prodsup.database.windows.net | SQL Auth |
| Production | sqls-bmp-prod.database.windows.net | SQL Auth |

## Applying Scripts Manually

### Via sqlcmd
```powershell
sqlcmd -S sqls-bmp-dev.database.windows.net -d sqldb-bmp-users-dev -U sqladmin -P 'YourPassword' -i users/001_InitialSchema.sql -C
```

### Via Azure Portal
1. Go to your Azure SQL Database → Query Editor
2. Paste and run the script
