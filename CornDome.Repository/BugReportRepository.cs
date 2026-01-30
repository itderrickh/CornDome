using CornDome.Models;

namespace CornDome.Repository
{
    public interface IBugReportRepository
    {
        Task<IEnumerable<BugReport>> GetBugReports();
        Task<bool> AddBugReport(BugReport bugReport);
        Task<bool> RemoveBugReport(BugReport bugReport);
        Task<bool> RemoveBugReport(int id);
    }

    public class BugReportRepository(MainContext context) : IBugReportRepository
    {
        public async Task<bool> AddBugReport(BugReport bugReport)
        {
            try
            {
                context.BugReports.Add(bugReport);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<BugReport>> GetBugReports()
        {
            return context.BugReports;
        }

        public async Task<bool> RemoveBugReport(int id)
        {
            try
            {
                var br = context.BugReports.FirstOrDefault(x => x.Id == id);
                if (br != null)
                {
                    context.Remove(br);
                    await context.SaveChangesAsync();
                    return true;
                }                
            }
            catch
            {
                return false;
            }

            return false;
        }

        public async Task<bool> RemoveBugReport(BugReport bugReport)
        {
            try
            {
                context.Remove(bugReport);
                await context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
