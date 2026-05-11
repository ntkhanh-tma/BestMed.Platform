using Azure.Messaging.ServiceBus;
using BestMed.Common.Messaging;
using BestMed.Platform.ServiceDefaults.Messaging;
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
        builder.Services.AddScoped<IEventHandler<TEvent>, THandler>();

        var topicName = ToTopicName(typeof(TEvent).Name);

        builder.Services.AddSingleton<IHostedService>(sp =>
            new ServiceBusSubscriberWorker<TEvent>(
                sp.GetRequiredService<ServiceBusClient>(),
                sp.GetRequiredService<IServiceScopeFactory>(),
                topicName,
                subscriptionName,
                sp.GetRequiredService<ILogger<ServiceBusSubscriberWorker<TEvent>>>()));

        return builder;
    }

    private static string ToTopicName(string eventTypeName)
    {
        var withoutSuffix = eventTypeName.EndsWith("Event", StringComparison.Ordinal)
            ? eventTypeName[..^5]
            : eventTypeName;

        return System.Text.RegularExpressions.Regex
            .Replace(withoutSuffix, "(?<!^)([A-Z])", "-$1")
            .ToLowerInvariant();
    }
}
