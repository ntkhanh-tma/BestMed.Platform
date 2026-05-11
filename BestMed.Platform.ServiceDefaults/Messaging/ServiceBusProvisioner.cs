using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.Platform.ServiceDefaults.Messaging;

/// <summary>
/// A hosted service that runs early in the startup pipeline to ensure all required
/// Service Bus topics and subscriptions exist before publishers/subscribers attempt to use them.
/// </summary>
internal sealed class ServiceBusProvisioner(
    ServiceBusAdministrationClient adminClient,
    IEnumerable<TopicSubscriptionRegistration> registrations,
    ILogger<ServiceBusProvisioner> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // Deduplicate topics
        var topics = registrations.Select(r => r.TopicName).Distinct().ToList();

        foreach (var topic in topics)
        {
            try
            {
                if (!await adminClient.TopicExistsAsync(topic, cancellationToken))
                {
                    await adminClient.CreateTopicAsync(topic, cancellationToken);
                    logger.LogInformation("Created Service Bus topic '{Topic}'", topic);
                }
                else
                {
                    logger.LogDebug("Service Bus topic '{Topic}' already exists", topic);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure Service Bus topic '{Topic}' exists. Messaging may fail at runtime.", topic);
            }
        }

        // Ensure subscriptions
        foreach (var reg in registrations.Where(r => r.SubscriptionName is not null))
        {
            try
            {
                if (!await adminClient.SubscriptionExistsAsync(reg.TopicName, reg.SubscriptionName!, cancellationToken))
                {
                    await adminClient.CreateSubscriptionAsync(reg.TopicName, reg.SubscriptionName!, cancellationToken);
                    logger.LogInformation("Created Service Bus subscription '{Subscription}' on topic '{Topic}'", reg.SubscriptionName, reg.TopicName);
                }
                else
                {
                    logger.LogDebug("Service Bus subscription '{Subscription}' on topic '{Topic}' already exists", reg.SubscriptionName, reg.TopicName);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to ensure subscription '{Subscription}' on topic '{Topic}'. Messaging may fail at runtime.", reg.SubscriptionName, reg.TopicName);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}

/// <summary>
/// Describes a topic (and optionally a subscription) that must exist before the service starts processing messages.
/// </summary>
internal sealed record TopicSubscriptionRegistration(string TopicName, string? SubscriptionName = null);
