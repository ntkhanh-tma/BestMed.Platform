using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
using BestMed.WarehouseService.Data;

namespace BestMed.WarehouseService;

/// <summary>
/// Extension methods to register WarehouseService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddWarehouseServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.WarehouseDb, ServiceNames.SchemaScripts.Warehouses);

        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<WarehouseDbContext>(ServiceNames.ConnectionStrings.WarehouseDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyWarehouseDbContext>(ServiceNames.ConnectionStrings.WarehouseDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<WarehouseUpdatedEvent>();

        return builder;
    }
}
