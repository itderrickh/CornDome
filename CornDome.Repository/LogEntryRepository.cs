using CornDome.Models;

namespace CornDome.Repository
{
    public interface ILogEntryRepository
    {
        Task InsertAsync(LogEntry log);
        Task<IEnumerable<LogEntry>> GetLogs();
    }

    public class LogEntryRepository(MainContext context) : ILogEntryRepository
    {
        public async Task InsertAsync(LogEntry log)
        {
            context.LogEntries.Add(log);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogs()
        {
            return context.LogEntries
                .OrderByDescending(x => x.Timestamp)
                .Take(500);
        }
    }
}
