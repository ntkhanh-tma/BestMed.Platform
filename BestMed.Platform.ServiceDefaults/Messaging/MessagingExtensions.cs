using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Azure.Identity;
using BestMed.Common.Messaging;
using BestMed.Platform.ServiceDefaults.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.Hosting;

public static class MessagingExtensions
{
    /// <summary>
    /// Registers the <see cref="IEventPublisher"/> backed by Azure Service Bus.
    /// Call this in ServiceRegistration.cs for services that publish events.
    /// </summary>
    public static IHostApplicationBuilder AddServiceBusPublisher(
        this IHostApplicationBuilder builder,
        string connectionName = "servicebus")
    {
        builder.AddAzureServiceBusClient(connectionName);
        EnsureProvisionerRegistered(builder, connectionName);
        builder.Services.AddSingleton<IEventPublisher, ServiceBusEventPublisher>();
        return builder;
    }

    /// <summary>
    /// Registers a background subscriber for <typeparamref name="TEvent"/> and its handler.
    /// Topic name is derived automatically from the event type name (e.g. RoleUpdatedEvent → role-updated).
    /// </summary>
    /// <param name="subscriptionName">
    /// The Service Bus topic subscription name for this service
    /// (e.g. "userservice-role-updated").
    /// </param>
    public static IHostApplicationBuilder AddServiceBusSubscriber<TEvent, THandler>(
        this IHostApplicationBuilder builder,
        string subscriptionName,
        string connectionName = "servicebus")
        where TEvent : IIntegrationEvent
        where THandler : class, IEventHandler<TEvent>
    {
        builder.AddAzureServiceBusClient(connectionName);
        EnsureProvisionerRegistered(builder, connectionName);
        builder.Services.AddScoped<IEventHandler<TEvent>, THandler>();

        var topicName = ToTopicName(typeof(TEvent).Name);

        // Register provisioning metadata so the provisioner creates this topic + subscription on startup
        builder.Services.AddSingleton(new TopicSubscriptionRegistration(topicName, subscriptionName));

        builder.Services.AddSingleton<IHostedService>(sp =>
            new ServiceBusSubscriberWorker<TEvent>(
                sp.GetRequiredService<ServiceBusClient>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                topicName,
                subscriptionName,
                sp.GetRequiredService<ILogger<ServiceBusSubscriberWorker<TEvent>>>()));

        return builder;
    }

    /// <summary>
    /// Registers a topic that this service publishes to, ensuring it is created on startup.
    /// Call this after <see cref="AddServiceBusPublisher"/> for each event type published.
    /// </summary>
    public static IHostApplicationBuilder EnsureTopicExists<TEvent>(this IHostApplicationBuilder builder)
        where TEvent : IIntegrationEvent
    {
        var topicName = ToTopicName(typeof(TEvent).Name);
        builder.Services.AddSingleton(new TopicSubscriptionRegistration(topicName));
        return builder;
    }

    private static void EnsureProvisionerRegistered(IHostApplicationBuilder builder, string connectionName = "servicebus")
    {
        // Use a marker to avoid duplicate registrations
        if (builder.Services.Any(s => s.ServiceType == typeof(ServiceBusProvisionerMarker)))
            return;

        builder.Services.AddSingleton<ServiceBusProvisionerMarker>();

        // Resolve the connection string at registration time for the admin client
        var connectionString = builder.Configuration.GetConnectionString(connectionName);

        builder.Services.AddSingleton(_ =>
        {
            if (!string.IsNullOrWhiteSpace(connectionString))
                return new ServiceBusAdministrationClient(connectionString);

            // Fallback to DefaultAzureCredential for managed identity scenarios
            var client = _.GetRequiredService<ServiceBusClient>();
            return new ServiceBusAdministrationClient(
                client.FullyQualifiedNamespace,
                new DefaultAzureCredential());
        });

        // The provisioner runs before subscriber workers because hosted services start in registration order
        builder.Services.AddHostedService<ServiceBusProvisioner>();
    }

    private static string ToTopicName(string eventTypeName)
    {
        var withoutSuffix = eventTypeName.EndsWith("Event", StringComparison.Ordinal)
            ? eventTypeName[..^5]
            : eventTypeName;

        return "bmp-" + System.Text.RegularExpressions.Regex
            .Replace(withoutSuffix, "(?<!^)([A-Z])", "-$1")
            .ToLowerInvariant();
    }
}

/// <summary>Marker to prevent duplicate provisioner registrations.</summary>
internal sealed class ServiceBusProvisionerMarker;
