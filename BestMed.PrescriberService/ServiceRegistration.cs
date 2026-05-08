using BestMed.PrescriberService.Data;

namespace BestMed.PrescriberService;

/// <summary>
/// Extension methods to register PrescriberService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddPrescriberServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<PrescriberDbContext>("prescriberdb", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyPrescriberDbContext>("prescriberdb-readonly", configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Service Bus publisher: notifies other services when prescriber data changes.
        builder.AddServiceBusPublisher();

        return builder;
    }
}

