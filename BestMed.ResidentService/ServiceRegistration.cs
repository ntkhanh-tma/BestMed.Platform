using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
using BestMed.ResidentService.Data;
using BestMed.ResidentService.Services;

namespace BestMed.ResidentService;

/// <summary>
/// Extension methods to register ResidentService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddResidentServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.ResidentDb, ServiceNames.SchemaScripts.Residents);

        // Read-write context: used for write operations.
        builder.AddSqlServerDbContext<ResidentDbContext>(ServiceNames.ConnectionStrings.ResidentDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for the read replica.
        builder.AddSqlServerDbContext<ReadOnlyResidentDbContext>(ServiceNames.ConnectionStrings.ResidentDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<ResidentUpdatedEvent>();

        builder.Services.AddScoped<IResidentService, Services.ResidentService>();
        builder.Services.AddScoped<IVmcService, VmcService>();

        // HTTP client for inter-service calls to FacilityService (exists).
        builder.Services.AddHttpClient<IFacilityClient, FacilityClient>(client =>
        {
            client.BaseAddress = new Uri(ServiceNames.ServiceUrl(ServiceNames.FacilityService));
        });

        // Null-object stubs for services not yet built — replace each with a real HTTP client
        // (AddHttpClient<IXxxClient, XxxClient>(...)) when the sibling service exists.
        builder.Services.AddScoped<IDrugClient, NullDrugClient>();
        builder.Services.AddScoped<IMedicationTrackingClient, NullMedicationTrackingClient>();
        builder.Services.AddScoped<IObservationsClient, NullObservationsClient>();
        builder.Services.AddScoped<IOrderingClient, NullOrderingClient>();

        return builder;
    }
}
