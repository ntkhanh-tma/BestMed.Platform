using Azure.Messaging.ServiceBus;
using BestMed.Common.Messaging;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace BestMed.Platform.ServiceDefaults.Messaging;

/// <summary>
/// Publishes integration events to Azure Service Bus topics.
/// Topic name is derived from the event class name:
///   RoleUpdatedEvent → role-updated
/// </summary>
internal sealed class ServiceBusEventPublisher(
    ServiceBusClient client,
    ILogger<ServiceBusEventPublisher> logger) : IEventPublisher
{
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IIntegrationEvent
    {
        var topicName = ToTopicName(typeof(TEvent).Name);
        var sender = client.CreateSender(topicName);

        var body = JsonSerializer.Serialize(@event, @event.GetType());
        var message = new ServiceBusMessage(body)
        {
            MessageId = @event.EventId.ToString(),
            ContentType = "application/json",
            Subject = typeof(TEvent).Name
        };

        try
        {
            await sender.SendMessageAsync(message, cancellationToken);
            logger.LogInformation("Published {EventType} (EventId={EventId}) to topic {Topic}",
                typeof(TEvent).Name, @event.EventId, topicName);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to publish {EventType} (EventId={EventId}) to topic {Topic}",
                typeof(TEvent).Name, @event.EventId, topicName);
            throw;
        }
        finally
        {
            await sender.DisposeAsync();
        }
    }

    /// <summary>Converts PascalCase event name to kebab-case topic name, stripping the "Event" suffix.</summary>
    private static string ToTopicName(string eventTypeName)
    {
        var withoutSuffix = eventTypeName.EndsWith("Event", StringComparison.Ordinal)
            ? eventTypeName[..^5]
            : eventTypeName;

        return "bmp-" + Regex.Replace(withoutSuffix, "(?<!^)([A-Z])", "-$1").ToLowerInvariant();
    }
}
