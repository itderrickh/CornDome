using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class ErrorLogsModel(ILogEntryRepository repository) : BasePageModel
    {
        public IEnumerable<LogEntry> Logs { get; set; } = [];

        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public async Task OnGetAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            PageNumber = Math.Max(1, pageNumber);
            PageSize = Math.Clamp(pageSize, 1, 500);
            var result = await repository.GetLogs(PageNumber, PageSize);

            Logs = result.Items;
            TotalCount = result.TotalCount;
        }
    }
}
