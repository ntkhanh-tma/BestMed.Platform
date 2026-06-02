using Microsoft.Extensions.Logging;

namespace BestMed.Platform.Tests;

/// <summary>
/// Custom [Fact] attribute that skips the test unless RUN_INTEGRATION_TESTS=true is set.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class IntegrationFactAttribute : FactAttribute
{
    private static readonly bool Enabled =
        string.Equals(Environment.GetEnvironmentVariable("RUN_INTEGRATION_TESTS"), "true", StringComparison.OrdinalIgnoreCase);

    public IntegrationFactAttribute()
    {
        if (!Enabled)
            Skip = "Set RUN_INTEGRATION_TESTS=true to run Aspire integration tests.";
    }
}

/// <summary>
/// Aspire-based integration smoke tests.
/// Skipped by default — require live Azure SQL and Azure Service Bus.
///
/// To enable locally:
///   $env:RUN_INTEGRATION_TESTS = "true"
///   dotnet test BestMed.Platform.Tests --filter "Category=Integration"
/// </summary>
public class WebTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);

    [IntegrationFact]
    [Trait("Category", "Integration")]
    public async Task GatewayHealthEndpointReturnsHealthyStatus()
    {
        // Arrange
        var cancellationToken = TestContext.Current.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.BestMed_Platform_AppHost>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        // Act
        var httpClient = app.CreateHttpClient("gateway");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("gateway", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        var response = await httpClient.GetAsync("/health", cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
