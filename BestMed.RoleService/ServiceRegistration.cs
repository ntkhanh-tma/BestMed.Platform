using BestMed.RoleService.Data;

namespace BestMed.RoleService;

/// <summary>
/// Extension methods to register RoleService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddRoleServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<RoleDbContext>("roledb", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyRoleDbContext>("roledb-readonly", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Service Bus publisher: notifies other services when role data changes.
        builder.AddServiceBusPublisher();

        return builder;
    }
}

