using CornDome.Models.Main;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Main
{
    public interface INoticeRepository
    {
        Task<Notice> GetActiveNoticeAsync();
        Task CancelAsync(int id);
        Task<List<Notice>> GetAllAsync();
        Task<Notice> PublishAsync(int id);
        Task CreateAsync(Notice notice);
        Task<List<Notice>> FindStartingAnnouncements(DateTime now, CancellationToken cancellationToken);
        Task<List<Notice>> EndAnnouncements(DateTime now, CancellationToken cancellationToken);
    }

    public class NoticeRepository(MainContext context) : INoticeRepository
    {
        public async Task<Notice> GetActiveNoticeAsync()
        {
            var now = DateTime.UtcNow;

            return await context.Notices
                .Where(x => x.IsPublished && x.IsActive && x.ScheduledStart > now)
                .OrderBy(x => x.ScheduledStart)
                .FirstOrDefaultAsync();
        }

        public async Task CancelAsync(int id)
        {
            var notice = await context.Notices.FindAsync(id);

            if (notice == null)
                return;

            notice.IsPublished = false;
            notice.IsActive = false;

            await context.SaveChangesAsync();
        }

        public async Task<List<Notice>> GetAllAsync()
        {
            return await context.Notices
                .OrderByDescending(x => x.ScheduledStart)
                .ToListAsync();
        }

        public async Task CreateAsync(Notice notice)
        {
            context.Notices.Add(notice);
            await context.SaveChangesAsync();
        }

        public async Task<List<Notice>> FindStartingAnnouncements(DateTime now, CancellationToken cancellationToken)
        {
            return await context.Notices
                .Where(x => x.IsPublished && x.IsActive && x.ScheduledStart <= now && x.ScheduledEnd > now)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Notice>> EndAnnouncements(DateTime now, CancellationToken cancellationToken)
        {
            var ending = await context.Notices
                .Where(x => x.IsPublished && x.IsActive && x.ScheduledEnd <= now)
                .ToListAsync(cancellationToken);

            foreach (var notice in ending)
            {
                notice.IsActive = false;
            }

            if (ending.Count > 0)
            {
                await context.SaveChangesAsync(cancellationToken);
            }

            return ending;
        }

        public async Task<Notice> PublishAsync(int id)
        {
            var notice = await context.Notices
                .FirstOrDefaultAsync(x => x.Id == id);

            if (notice == null || !notice.IsActive)
                return null;

            notice.IsPublished = true;

            await context.SaveChangesAsync();
            return notice;
        }
    }
}
