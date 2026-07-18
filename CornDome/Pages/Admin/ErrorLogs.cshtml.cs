using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    public class ErrorLogsModel(ILogEntryRepository repository) : PageModel
    {
        public IEnumerable<LogEntry> Logs { get; set; } = [];

        public async Task OnGetAsync()
        {
            Logs = await repository.GetLogs();
        }
    }
}
