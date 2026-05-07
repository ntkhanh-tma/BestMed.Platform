# Database Scripts

This folder contains the SQL scripts for the BestMed database schema.
Scripts are numbered in order of execution.

## Database-First Workflow

1. **Modify schema** — Create a new numbered SQL script (e.g., `002_AddColumnX.sql`)
2. **Apply to database** — Run the script against the target Azure SQL database
3. **Re-scaffold entities** — From the `BestMed.UserService` project directory:

```powershell
dotnet ef dbcontext scaffold "Name=ConnectionStrings:userdb" Microsoft.EntityFrameworkCore.SqlServer `
    --output-dir Entities `
    --context-dir Data `
    --context UserDbContext `
    --force `
    --no-onconfiguring
```

4. **Update DTOs and mappings** — If new columns were added, update the DTOs in `DTOs/` and mapping in `Mapping/UserMappingExtensions.cs`

## Environments

| Environment | Server | Database | Auth |
|---|---|---|---|
| Dev | bestmed-dev.database.windows.net | BestMedUsers | Managed Identity |
| BAT | bestmed-bat.database.windows.net | BestMedUsers | Managed Identity |
| Prodsup | bestmed-prodsup.database.windows.net | BestMedUsers | Managed Identity |
| Production | bestmed-prod.database.windows.net | BestMedUsers | Managed Identity |

All environments, including local developer debugging, connect to Azure SQL using
Managed Identity (`Active Directory Default`). There is no local SQL container.

## Applying Scripts

### Via Azure CLI
```bash
sqlcmd -S bestmed-dev.database.windows.net -d BestMedUsers --authentication-method=ActiveDirectoryDefault -i 001_InitialSchema.sql
```

### Via Azure Portal
1. Go to your Azure SQL Database → Query Editor
2. Paste and run the script
