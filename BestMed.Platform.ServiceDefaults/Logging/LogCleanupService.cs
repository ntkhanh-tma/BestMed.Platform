using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BestMed.Platform.ServiceDefaults.Logging;

/// <summary>
/// Configuration options for the <see cref="LogCleanupService"/>.
/// </summary>
public sealed class LogCleanupOptions
{
    /// <summary>Directory where log files are stored.</summary>
    public string LogDirectory { get; set; } = "logs";

    /// <summary>Maximum number of log files to retain. Older files are deleted.</summary>
    public int RetainedFileCount { get; set; } = 30;

    /// <summary>How often the cleanup runs.</summary>
    public TimeSpan CleanupInterval { get; set; } = TimeSpan.FromHours(24);
}

/// <summary>
/// A background service that periodically removes old log files beyond the configured retention limit.
/// This acts as a safety net in addition to Serilog's built-in <c>retainedFileCountLimit</c>,
/// ensuring disk space is reclaimed even if multiple rolling files accumulate.
/// </summary>
internal sealed class LogCleanupService(
    LogCleanupOptions options,
    ILogger<LogCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Delay initial cleanup to let the service finish starting
        await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                CleanupOldLogs();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during log file cleanup");
            }

            await Task.Delay(options.CleanupInterval, stoppingToken);
        }
    }

    private void CleanupOldLogs()
    {
        if (!Directory.Exists(options.LogDirectory))
            return;

        var logFiles = Directory.GetFiles(options.LogDirectory, "*.json")
            .Concat(Directory.GetFiles(options.LogDirectory, "*.log"))
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTimeUtc)
            .ToList();

        if (logFiles.Count <= options.RetainedFileCount)
            return;

        var filesToDelete = logFiles.Skip(options.RetainedFileCount);
        var deletedCount = 0;

        foreach (var file in filesToDelete)
        {
            try
            {
                file.Delete();
                deletedCount++;
            }
            catch (IOException ex)
            {
                logger.LogDebug(ex, "Could not delete log file {File} (may be in use)", file.Name);
            }
        }

        if (deletedCount > 0)
        {
            logger.LogInformation("Log cleanup removed {Count} old log file(s) from {Directory}",
                deletedCount, options.LogDirectory);
        }
    }
}
