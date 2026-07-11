using CareGuide.Core.Interfaces;
using CareGuide.Models.Configuration;
using Microsoft.Extensions.Options;

namespace CareGuide.API.BackgroundServices;

public sealed class AbandonedUploadCleanupBackgroundService(
    IServiceScopeFactory scopeFactory,
    IOptions<FileLifecycleOptions> options,
    ILogger<AbandonedUploadCleanupBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(options.Value.CleanupIntervalMinutes));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var cleanup = scope.ServiceProvider.GetRequiredService<IAbandonedUploadCleanupService>();
                var deleted = await cleanup.CleanupAsync(stoppingToken);
                if (deleted > 0) logger.LogInformation("Removed {DeletedCount} abandoned pending uploads.", deleted);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Abandoned upload cleanup failed and will be retried.");
            }
        }
    }
}
