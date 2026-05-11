using BestMed.Common.Constants;
using BestMed.Common.Messaging.Events;
using BestMed.Data;
using BestMed.UserService.Clients;
using BestMed.UserService.Data;
using BestMed.UserService.Messaging;

namespace BestMed.UserService;

/// <summary>
/// Extension methods to register UserService-specific services.
/// Keeps Program.cs minimal and focused on the application pipeline.
/// </summary>
public static class ServiceRegistration
{
    public static IHostApplicationBuilder AddUserServiceDefaults(this IHostApplicationBuilder builder)
    {
        // Auto-provision database on startup (Development only)
        builder.AddDatabaseInitializer(ServiceNames.ConnectionStrings.UserDb, ServiceNames.SchemaScripts.Users);

        // Read-write context: used for create/update/delete operations.
        builder.AddSqlServerDbContext<UserDbContext>(ServiceNames.ConnectionStrings.UserDb, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // Read-only context: uses a separate connection string for read replica.
        builder.AddSqlServerDbContext<ReadOnlyUserDbContext>(ServiceNames.ConnectionStrings.UserDbReadOnly, configureSettings: settings =>
        {
            settings.DisableHealthChecks = true;
        });

        // In-memory cache for cross-service data (roles, prescribers).
        builder.Services.AddMemoryCache();

        // ── HTTP clients (synchronous, for live queries) ──────────────────────────
        // Used when UserService needs role/prescriber data as part of handling a request.
        // Aspire service discovery resolves the addresses; resilience handler adds retries.

        builder.Services.AddHttpClient<RoleServiceClient>(client =>
            client.BaseAddress = new Uri(ServiceNames.ServiceUrl(ServiceNames.RoleService)));

        builder.Services.AddHttpClient<PrescriberServiceClient>(client =>
            client.BaseAddress = new Uri(ServiceNames.ServiceUrl(ServiceNames.PrescriberService)));

        // Register caching decorators as the resolved interface.
        builder.Services.AddSingleton<IRoleServiceClient>(sp =>
            new CachingRoleServiceClient(
                sp.GetRequiredService<RoleServiceClient>(),
                sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));

        builder.Services.AddSingleton<IPrescriberServiceClient>(sp =>
            new CachingPrescriberServiceClient(
                sp.GetRequiredService<PrescriberServiceClient>(),
                sp.GetRequiredService<Microsoft.Extensions.Caching.Memory.IMemoryCache>()));

        // ── Service Bus ───────────────────────────────────────────────────────────
        // Publisher: UserService publishes UserStatusChangedEvent when a user's active status changes.
        // Pattern: Service Bus (async) — no synchronous consumer; downstream services react at their own pace.
        builder.AddServiceBusPublisher();
        builder.EnsureTopicExists<UserStatusChangedEvent>();

        // ── Service Bus (asynchronous, for cache invalidation) ────────────────────
        // When RoleService or PrescriberService publish a change event, UserService
        // invalidates its local cache so the next HTTP call fetches fresh data.

        builder.AddServiceBusSubscriber<RoleUpdatedEvent, RoleUpdatedEventHandler>(
            subscriptionName: ServiceNames.Subscriptions.UserServiceRoleUpdated);

        builder.AddServiceBusSubscriber<PrescriberUpdatedEvent, PrescriberUpdatedEventHandler>(
            subscriptionName: ServiceNames.Subscriptions.UserServicePrescriberUpdated);

        return builder;
    }
}

