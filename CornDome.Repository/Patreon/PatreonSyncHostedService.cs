
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CornDome.Repository.Patreon
{
    public class PatreonSyncHostedService(
        IServiceScopeFactory scopeFactory,
        ILogger<PatreonSyncHostedService> logger) : BackgroundService
    {
        private static readonly TimeSpan SyncInterval = TimeSpan.FromHours(6);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Give the application a moment to finish starting up.
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();

                    var syncService = scope.ServiceProvider.GetRequiredService<IPatreonSyncService>();

                    await syncService.SyncAllAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Patreon background synchronization failed.");
                }

                await Task.Delay(SyncInterval, stoppingToken);
            }
        }
    }
}
