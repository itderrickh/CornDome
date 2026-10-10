using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class BugManagementModel(IBugReportRepository bugReportRepository) : BasePageModel
    {
        public List<BugReport> Bugs { get; set; }

        [BindProperty]
        public int RemoveBugId { get; set; }

        public async void OnGet()
        {
            await PopulateTables();
        }

        private async Task PopulateTables()
        {
            var bugs = await bugReportRepository.GetBugReports();
            Bugs = bugs.ToList();
        }


        public async Task<IActionResult> OnPostRemoveBug()
        {
            await PopulateTables();

            if (RemoveBugId > -1)
            {
                var isSuccess = await bugReportRepository.RemoveBugReport(RemoveBugId);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Bug squash succeeded";
                    await PopulateTables();
                    return Page();
                }
            }

            await PopulateTables();
            TempData["ErrorMessage"] = "The Bug Grew Bigger :O";
            return Page();
        }
    }
}
