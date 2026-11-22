using CornDome.Hubs;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.SignalR;

namespace CornDome.Services
{
    public class NoticeBackgroundService(IHubContext<NoticeHub> hub, ILogger<NoticeBackgroundService> logger, IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckMaintenanceAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error checking scheduled maintenance.");
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }
        }

        private async Task CheckMaintenanceAsync(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateScope();

            var noticeRepository = scope.ServiceProvider.GetRequiredService<INoticeRepository>();
            var now = DateTime.UtcNow;

            // Find announcements that should start.
            var startingNotices = await noticeRepository.FindStartingAnnouncements(now, cancellationToken);

            foreach (var notice in startingNotices)
            {
                await hub.Clients.All.SendAsync("MaintenanceStarted", notice, cancellationToken);

                logger.LogInformation("Maintenance started: {Title}", notice.Title);
            }

            // Find announcements that have ended.
            var endedNotices = await noticeRepository.EndAnnouncements(now, cancellationToken);

            foreach (var notice in endedNotices)
            {
                await hub.Clients.All.SendAsync("MaintenanceEnded", notice.Id, cancellationToken);

                logger.LogInformation("Maintenance ended: {Title}", notice.Title);
            }
        }
    }
}
