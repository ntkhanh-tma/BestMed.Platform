using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.Platform.ServiceDefaults.Messaging;

/// <summary>
/// A hosted lifecycle service that ensures all required Service Bus topics and subscriptions
/// exist. Runs after the host has started so health checks can respond during provisioning.
/// </summary>
internal sealed class ServiceBusProvisioner(
    ServiceBusAdministrationClient adminClient,
    IEnumerable<TopicSubscriptionRegistration> registrations,
    ILogger<ServiceBusProvisioner> logger) : IHostedLifecycleService
{
    public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StartingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppingAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    public Task StoppedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public async Task StartedAsync(CancellationToken cancellationToken)
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
}

/// <summary>
/// Describes a topic (and optionally a subscription) that must exist before the service starts processing messages.
/// </summary>
internal sealed record TopicSubscriptionRegistration(string TopicName, string? SubscriptionName = null);
