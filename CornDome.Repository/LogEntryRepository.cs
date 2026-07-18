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
        private readonly MainContext _context = context;

        public async Task InsertAsync(LogEntry log)
        {
            _context.LogEntries.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogEntry>> GetLogs()
        {
            return context.LogEntries
                .OrderByDescending(x => x.Timestamp)
                .Take(500);
        }
    }
}
