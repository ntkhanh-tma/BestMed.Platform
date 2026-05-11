using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
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
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.PrescriberDb, ServiceNames.SchemaScripts.Prescribers);

        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<PrescriberDbContext>(ServiceNames.ConnectionStrings.PrescriberDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyPrescriberDbContext>(ServiceNames.ConnectionStrings.PrescriberDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<PrescriberUpdatedEvent>();

        return builder;
    }
}

