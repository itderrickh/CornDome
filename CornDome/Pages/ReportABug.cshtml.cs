using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class BugReportVm
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [StringLength(2000)]
        public string Description { get; set; }

        [StringLength(2000)]
        public string Steps { get; set; }
    }

    public class ReportABugModel(IBugReportRepository bugReportRepository) : PageModel
    {
        [BindProperty]
        public BugReportVm Bug { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();
            int? userId = User.Identity?.IsAuthenticated == true ? int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) : null;

            var report = new BugReport
            {
                UserId = userId,
                Title = Bug.Title,
                Description = Bug.Description,
                Steps = Bug.Steps,
                CreatedAt = DateTime.UtcNow
            };

            await bugReportRepository.AddBugReport(report);

            return RedirectToPage("/ReportBugSuccess");
        }
    }
}
