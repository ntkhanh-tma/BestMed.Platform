using Azure.Messaging.ServiceBus;
using BestMed.Common.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BestMed.Platform.ServiceDefaults.Messaging;

/// <summary>
/// Background service that listens to a Service Bus topic subscription and dispatches
/// received messages to the registered <see cref="IEventHandler{TEvent}"/>.
/// One instance is created per (topic, subscription, event-type) registration.
/// </summary>
internal sealed class ServiceBusSubscriberWorker<TEvent>(
    ServiceBusClient client,
    IServiceScopeFactory scopeFactory,
    string topicName,
    string subscriptionName,
    ILogger<ServiceBusSubscriberWorker<TEvent>> logger) : BackgroundService
    where TEvent : IIntegrationEvent
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var processor = client.CreateProcessor(topicName, subscriptionName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = false,
            MaxConcurrentCalls = 4
        });

        processor.ProcessMessageAsync += OnMessageAsync;
        processor.ProcessErrorAsync += OnErrorAsync;

        await processor.StartProcessingAsync(stoppingToken);
        logger.LogInformation("Subscribed to topic '{Topic}' / subscription '{Subscription}' for {EventType}",
            topicName, subscriptionName, typeof(TEvent).Name);

        try
        {
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        finally
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }

    private async Task OnMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var @event = JsonSerializer.Deserialize<TEvent>(args.Message.Body.ToString());
            if (@event is null)
            {
                logger.LogWarning("Received null or undeserializable message on topic '{Topic}'", topicName);
                await args.DeadLetterMessageAsync(args.Message, "NullPayload", "Message body deserialized to null");
                return;
            }

            using var scope = scopeFactory.CreateScope();
            var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEvent>>();
            await handler.HandleAsync(@event, args.CancellationToken);

            await args.CompleteMessageAsync(args.Message);
            logger.LogInformation("Handled {EventType} (EventId={EventId})",
                typeof(TEvent).Name, @event.EventId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling {EventType} from topic '{Topic}'", typeof(TEvent).Name, topicName);
            await args.AbandonMessageAsync(args.Message);
        }
    }

    private Task OnErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(args.Exception,
            "Service Bus processor error on topic '{Topic}' / subscription '{Subscription}': {ErrorSource}",
            topicName, subscriptionName, args.ErrorSource);
        return Task.CompletedTask;
    }
}
