using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
using BestMed.PharmacyService.Data;
using BestMed.PharmacyService.Services;

namespace BestMed.PharmacyService;

/// <summary>
/// Extension methods to register PharmacyService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddPharmacyServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.PharmacyDb, ServiceNames.SchemaScripts.Pharmacies);

        // Read-write context: used for update operations.
        builder.AddSqlServerDbContext<PharmacyDbContext>(ServiceNames.ConnectionStrings.PharmacyDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyPharmacyDbContext>(ServiceNames.ConnectionStrings.PharmacyDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<PharmacyUpdatedEvent>();

        builder.Services.AddScoped<IPharmacyService, Services.PharmacyService>();

        return builder;
    }
}
