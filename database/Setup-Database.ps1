<#
.SYNOPSIS
    Provisions an Azure SQL database for a BestMed Platform service.

.DESCRIPTION
    Creates a new Azure SQL database on the specified server, creates a SQL login
    and database user with db_datareader + db_datawriter roles, and optionally runs
    the service's initial schema script.

    Database naming convention: sqldb-bmp-{service}-{environment}
    Server naming convention:   sqls-bmp-{environment}

.PARAMETER ServiceName
    Short name of the service (e.g. users, roles, prescribers, warehouses).
    Used to derive the database name and locate the schema script folder.

.PARAMETER Environment
    Target environment (e.g. dev, bat, prodsup, prod). Default: dev.

.PARAMETER SqlServer
    Azure SQL Server name (without .database.windows.net). Default: sqls-bmp-{Environment}.

.PARAMETER SqlAdminUser
    SQL Server admin username used to create the database and login.

.PARAMETER SqlAdminPassword
    SQL Server admin password.

.PARAMETER AppUser
    SQL login/user to create for the application. Default: bmp-{ServiceName}-app.

.PARAMETER AppPassword
    Password for the application SQL login.

.PARAMETER ServiceObjective
    Azure SQL pricing tier. Default: S0.

.PARAMETER ResourceGroup
    Azure resource group containing the SQL Server.

.PARAMETER SkipSchema
    If set, skips running the initial schema script.

.EXAMPLE
    # Create the users database on sqls-bmp-dev with a new app login
    .\Setup-Database.ps1 `
        -ServiceName users `
        -Environment dev `
        -SqlAdminUser sqladmin `
        -SqlAdminPassword 'YourAdminP@ss!' `
        -AppUser bmp-users-app `
        -AppPassword 'YourAppP@ss!' `
        -ResourceGroup rg-bmp-dev

.EXAMPLE
    # Provision all service databases at once
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
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ServiceName,
    [string]$Environment = "dev",
    [string]$SqlServer,
    [Parameter(Mandatory)][string]$SqlAdminUser,
    [Parameter(Mandatory)][string]$SqlAdminPassword,
    [string]$AppUser,
    [Parameter(Mandatory)][string]$AppPassword,
    [string]$ServiceObjective = "S0",
    [Parameter(Mandatory)][string]$ResourceGroup,
    [switch]$SkipSchema
)

$ErrorActionPreference = "Stop"

# Derive defaults
if (-not $SqlServer) { $SqlServer = "sqls-bmp-$Environment" }
if (-not $AppUser) { $AppUser = "bmp-$ServiceName-app" }

$DatabaseName = "sqldb-bmp-$ServiceName-$Environment"
$ServerFQDN = "$SqlServer.database.windows.net"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "BestMed Platform — Database Provisioning" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Server:       $ServerFQDN"
Write-Host "  Database:     $DatabaseName"
Write-Host "  App User:     $AppUser"
Write-Host "  Tier:         $ServiceObjective"
Write-Host "  Schema:       $(if ($SkipSchema) { 'Skipped' } else { "database/$ServiceName/001_InitialSchema.sql" })"
Write-Host ""

# ── Step 1: Create the database ──────────────────────────────────────────────
Write-Host "[1/4] Creating database '$DatabaseName' on '$SqlServer'..." -ForegroundColor Yellow

az sql db create `
    --resource-group $ResourceGroup `
    --server $SqlServer `
    --name $DatabaseName `
    --service-objective $ServiceObjective `
    --output none

if ($LASTEXITCODE -ne 0) { throw "Failed to create database." }
Write-Host "  Database created." -ForegroundColor Green

# ── Step 2: Create SQL login on master ───────────────────────────────────────
Write-Host "[2/4] Creating SQL login '$AppUser' on master..." -ForegroundColor Yellow

$createLoginSql = @"
IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = '$AppUser')
BEGIN
    CREATE LOGIN [$AppUser] WITH PASSWORD = '$AppPassword';
END
"@

sqlcmd -S $ServerFQDN -d master -U $SqlAdminUser -P $SqlAdminPassword -Q $createLoginSql -C

if ($LASTEXITCODE -ne 0) { throw "Failed to create SQL login." }
Write-Host "  Login created." -ForegroundColor Green

# ── Step 3: Create database user and assign roles ────────────────────────────
Write-Host "[3/4] Creating database user '$AppUser' with read/write roles..." -ForegroundColor Yellow

$createUserSql = @"
IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = '$AppUser')
BEGIN
    CREATE USER [$AppUser] FOR LOGIN [$AppUser];
END
ALTER ROLE db_datareader ADD MEMBER [$AppUser];
ALTER ROLE db_datawriter ADD MEMBER [$AppUser];
"@

sqlcmd -S $ServerFQDN -d $DatabaseName -U $SqlAdminUser -P $SqlAdminPassword -Q $createUserSql -C

if ($LASTEXITCODE -ne 0) { throw "Failed to create database user." }
Write-Host "  User created and roles assigned." -ForegroundColor Green

# ── Step 4: Run initial schema script ────────────────────────────────────────
if (-not $SkipSchema) {
    $scriptPath = Join-Path $PSScriptRoot "$ServiceName/001_InitialSchema.sql"

    if (Test-Path $scriptPath) {
        Write-Host "[4/4] Running schema script '$scriptPath'..." -ForegroundColor Yellow

        sqlcmd -S $ServerFQDN -d $DatabaseName -U $SqlAdminUser -P $SqlAdminPassword -i $scriptPath -C

        if ($LASTEXITCODE -ne 0) { throw "Failed to run schema script." }
        Write-Host "  Schema applied." -ForegroundColor Green
    }
    else {
        Write-Host "[4/4] No schema script found at '$scriptPath' — skipping." -ForegroundColor DarkYellow
    }
}
else {
    Write-Host "[4/4] Schema application skipped (SkipSchema flag)." -ForegroundColor DarkYellow
}

Write-Host ""
Write-Host "Done! Connection string for appsettings:" -ForegroundColor Cyan
Write-Host "  Server=$ServerFQDN;Database=$DatabaseName;User Id=$AppUser;Password=<password>;Encrypt=True;TrustServerCertificate=False;" -ForegroundColor White
Write-Host ""
