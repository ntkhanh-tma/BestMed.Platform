using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.Data;

/// <summary>
/// A hosted service that runs on startup to ensure the database exists and the schema is applied.
/// <para>
/// Behaviour:
/// <list type="number">
///   <item>Connects to the <c>master</c> database on the same server.</item>
///   <item>If the target database does not exist, creates it.</item>
///   <item>Runs all provided SQL scripts in order against the target database.</item>
/// </list>
/// </para>
/// <para>
/// Register via <see cref="DatabaseInitializerExtensions.AddDatabaseInitializer"/> in each service's
/// <c>ServiceRegistration</c>. Only runs in the <c>Development</c> environment by default.
/// </para>
/// </summary>
public sealed class DatabaseInitializer : IHostedService
{
    private readonly string _connectionString;
    private readonly string[] _schemaScripts;
    private readonly ILogger<DatabaseInitializer> _logger;
    private readonly IHostEnvironment _environment;

    public DatabaseInitializer(
        string connectionString,
        string[] schemaScripts,
        ILogger<DatabaseInitializer> logger,
        IHostEnvironment environment)
    {
        _connectionString = connectionString;
        _schemaScripts = schemaScripts;
        _logger = logger;
        _environment = environment;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogDebug("Database auto-provisioning skipped (environment: {Environment})", _environment.EnvironmentName);
            return;
        }

        try
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            var databaseName = builder.InitialCatalog;

            if (string.IsNullOrWhiteSpace(databaseName))
            {
                _logger.LogWarning("Database auto-provisioning skipped — no database name found in connection string");
                return;
            }

            _logger.LogInformation("Checking if database '{Database}' exists on '{Server}'...", databaseName, builder.DataSource);

            // Connect to master to check/create the database
            var masterConnectionString = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            }.ConnectionString;

            await using var masterConnection = new SqlConnection(masterConnectionString);
            await masterConnection.OpenAsync(cancellationToken);

            var exists = await DatabaseExistsAsync(masterConnection, databaseName, cancellationToken);

            if (exists)
            {
                _logger.LogInformation("Database '{Database}' already exists — skipping provisioning", databaseName);
                return;
            }

            _logger.LogInformation("Database '{Database}' not found — creating...", databaseName);
            await CreateDatabaseAsync(masterConnection, databaseName, cancellationToken);
            _logger.LogInformation("Database '{Database}' created successfully", databaseName);

            // Apply schema scripts
            if (_schemaScripts.Length > 0)
            {
                _logger.LogInformation("Applying {Count} schema script(s) to '{Database}'...", _schemaScripts.Length, databaseName);

                await using var dbConnection = new SqlConnection(_connectionString);
                await dbConnection.OpenAsync(cancellationToken);

                foreach (var script in _schemaScripts)
                {
                    _logger.LogInformation("Running schema script: {Script}", Path.GetFileName(script));
                    var sql = await File.ReadAllTextAsync(script, cancellationToken);

                    // Split on GO batches
                    foreach (var batch in SplitBatches(sql))
                    {
                        if (string.IsNullOrWhiteSpace(batch)) continue;

                        await using var command = new SqlCommand(batch, dbConnection);
                        command.CommandTimeout = 120;
                        await command.ExecuteNonQueryAsync(cancellationToken);
                    }
                }

                _logger.LogInformation("Schema scripts applied successfully to '{Database}'", databaseName);
            }
        }
        catch (SqlException ex)
        {
            _logger.LogError(ex, "Database auto-provisioning failed. Ensure the SQL Server is accessible and the credentials have dbcreator permissions. The service will continue starting without provisioning.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during database auto-provisioning. The service will continue starting.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static async Task<bool> DatabaseExistsAsync(SqlConnection connection, string databaseName, CancellationToken cancellationToken)
    {
        const string sql = "SELECT COUNT(1) FROM sys.databases WHERE name = @name";
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@name", databaseName);
        var result = await command.ExecuteScalarAsync(cancellationToken);
        return Convert.ToInt32(result) > 0;
    }

    private static async Task CreateDatabaseAsync(SqlConnection connection, string databaseName, CancellationToken cancellationToken)
    {
        // Database names cannot be parameterised — use QuoteName equivalent
        var safeName = databaseName.Replace("]", "]]");
        var sql = $"CREATE DATABASE [{safeName}]";
        await using var command = new SqlCommand(sql, connection);
        command.CommandTimeout = 60;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static IEnumerable<string> SplitBatches(string sql)
    {
        // Split on lines that contain only "GO" (case-insensitive), matching sqlcmd behaviour
        return System.Text.RegularExpressions.Regex
            .Split(sql, @"^\s*GO\s*$", System.Text.RegularExpressions.RegexOptions.Multiline | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
}

/// <summary>
/// Extension methods to register <see cref="DatabaseInitializer"/> in a service's DI container.
/// </summary>
public static class DatabaseInitializerExtensions
{
    /// <summary>
    /// Registers a <see cref="DatabaseInitializer"/> that ensures the database exists and schema is applied on startup.
    /// Only runs in the Development environment.
    /// </summary>
    /// <param name="builder">The host application builder.</param>
    /// <param name="connectionStringName">The name of the connection string in configuration (e.g. "userdb").</param>
    /// <param name="schemaScriptPaths">
    /// Paths to SQL script files relative to the solution root.
    /// Scripts are executed in the order provided.
    /// </param>
    public static IHostApplicationBuilder AddDatabaseInitializer(
        this IHostApplicationBuilder builder,
        string connectionStringName,
        params string[] schemaScriptPaths)
    {
        var connectionString = builder.Configuration.GetConnectionString(connectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return builder;
        }

        // Resolve script paths relative to the content root (project directory),
        // walking up to the solution root where the database/ folder lives.
        var solutionRoot = FindSolutionRoot(builder.Environment.ContentRootPath);
        var resolvedPaths = schemaScriptPaths
            .Select(p => Path.GetFullPath(Path.Combine(solutionRoot, p)))
            .Where(File.Exists)
            .ToArray();

        builder.Services.AddHostedService(sp =>
            new DatabaseInitializer(
                connectionString,
                resolvedPaths,
                sp.GetRequiredService<ILogger<DatabaseInitializer>>(),
                sp.GetRequiredService<IHostEnvironment>()));

        return builder;
    }

    private static string FindSolutionRoot(string startDir)
    {
        var dir = new DirectoryInfo(startDir);
        while (dir is not null)
        {
            if (dir.GetFiles("*.sln").Length > 0 || dir.GetFiles("*.slnx").Length > 0)
                return dir.FullName;
            dir = dir.Parent;
        }

        // Fallback: assume one level up from content root (project dir → solution dir)
        return Path.GetFullPath(Path.Combine(startDir, ".."));
    }
}
