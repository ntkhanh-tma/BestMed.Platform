using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BestMed.Platform.ServiceDefaults.Logging;

/// <summary>
/// Configures Serilog as the universal logging provider for BestMed Platform services.
/// <para>
/// Features:
/// <list type="bullet">
///   <item>Console sink for Aspire dashboard / terminal visibility</item>
///   <item>Rolling file sink (daily) with configurable size and retention</item>
///   <item>Structured JSON logs for machine parsing</item>
///   <item>Configurable minimum levels via appsettings <c>Serilog</c> section</item>
///   <item>Automatic log cleanup via <see cref="LogCleanupService"/></item>
/// </list>
/// </para>
/// <para>
/// Configuration (appsettings.json):
/// <code>
/// "Serilog": {
///   "MinimumLevel": {
///     "Default": "Information",
///     "Override": {
///       "Microsoft.AspNetCore": "Warning",
///       "Microsoft.EntityFrameworkCore": "Warning",
///       "System.Net.Http": "Warning"
///     }
///   }
/// },
/// "FileLogging": {
///   "Enabled": true,
///   "Path": "logs/log-.json",
///   "RetainedFileCountLimit": 30,
///   "FileSizeLimitMB": 50,
///   "CleanupIntervalHours": 24
/// }
/// </code>
/// </para>
/// </summary>
public static class PlatformLoggingExtensions
{
    /// <summary>
    /// Adds Serilog-based structured logging with console and file sinks.
    /// Call this from <c>AddServiceDefaults</c> so all services inherit the configuration.
    /// </summary>
    public static TBuilder AddPlatformLogging<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        var configuration = builder.Configuration;
        var fileLoggingSection = configuration.GetSection("FileLogging");

        var fileLoggingEnabled = fileLoggingSection.GetValue("Enabled", true);
        var logPath = fileLoggingSection.GetValue("Path", "logs/log-.json") ?? "logs/log-.json";
        var retainedFileCount = fileLoggingSection.GetValue("RetainedFileCountLimit", 30);
        var fileSizeLimitBytes = fileLoggingSection.GetValue("FileSizeLimitMB", 50) * 1_048_576L;
        var cleanupIntervalHours = fileLoggingSection.GetValue("CleanupIntervalHours", 24);

        builder.Services.AddSerilog((services, loggerConfig) =>
        {
            loggerConfig
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("ServiceName", builder.Environment.ApplicationName)
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}  {Message:lj}{NewLine}{Exception}");

            if (fileLoggingEnabled)
            {
                loggerConfig.WriteTo.File(
                    path: logPath,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: retainedFileCount,
                    fileSizeLimitBytes: fileSizeLimitBytes,
                    rollOnFileSizeLimit: true,
                    shared: false,
                    formatter: new Serilog.Formatting.Json.JsonFormatter());
            }
        });

        // Register cleanup service if file logging is enabled
        if (fileLoggingEnabled)
        {
            builder.Services.AddSingleton(new LogCleanupOptions
            {
                LogDirectory = Path.GetDirectoryName(Path.GetFullPath(logPath)) ?? "logs",
                RetainedFileCount = retainedFileCount,
                CleanupInterval = TimeSpan.FromHours(cleanupIntervalHours)
            });
            builder.Services.AddHostedService<LogCleanupService>();
        }

        return builder;
    }
}
