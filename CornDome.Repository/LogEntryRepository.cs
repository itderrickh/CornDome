using CornDome.Models;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository
{
    public interface ILogEntryRepository
    {
        Task InsertAsync(LogEntry log);
        Task<PagedResult<LogEntry>> GetLogs(int page, int pageSize);
    }

    public record PagedResult<T>(
        IEnumerable<T> Items,
        int TotalCount,
        int Page,
        int PageSize);

    public class LogEntryRepository(MainContext context) : ILogEntryRepository
    {
        public async Task InsertAsync(LogEntry log)
        {
            context.LogEntries.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task<PagedResult<LogEntry>> GetLogs(int page, int pageSize)
        {
            var query = context.LogEntries
                .AsNoTracking()
                .OrderByDescending(x => x.Timestamp);

            var totalCount = await query.CountAsync();

            var logs = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<LogEntry>(
                logs,
                totalCount,
                page,
                pageSize);
        }
    }
}
