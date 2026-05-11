using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
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
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.RoleDb, ServiceNames.SchemaScripts.Roles);

        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<RoleDbContext>(ServiceNames.ConnectionStrings.RoleDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyRoleDbContext>(ServiceNames.ConnectionStrings.RoleDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<RoleUpdatedEvent>();

        return builder;
    }
}

