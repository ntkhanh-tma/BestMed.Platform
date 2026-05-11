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
        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<WarehouseDbContext>("warehousedb", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyWarehouseDbContext>("warehousedb-readonly", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Service Bus publisher: notifies other services when warehouse data changes.
        builder.AddServiceBusPublisher();

        return builder;
    }
}
