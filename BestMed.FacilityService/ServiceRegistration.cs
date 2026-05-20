using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
using BestMed.FacilityService.Data;

namespace BestMed.FacilityService;

/// <summary>
/// Extension methods to register FacilityService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddFacilityServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.FacilityDb, ServiceNames.SchemaScripts.Facilities);

        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<FacilityDbContext>(ServiceNames.ConnectionStrings.FacilityDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyFacilityDbContext>(ServiceNames.ConnectionStrings.FacilityDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<FacilityUpdatedEvent>();

        return builder;
    }
}
