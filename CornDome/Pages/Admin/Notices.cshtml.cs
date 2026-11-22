using CornDome.Hubs;
using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel.DataAnnotations;

namespace CornDome.Pages.Admin
{
    public class NoticeInput
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        [Required]
        public DateTime ScheduledStart { get; set; }

        [Required]
        public DateTime ScheduledEnd { get; set; }

        public bool PublishImmediately { get; set; }
    }

    [Authorize(Policy = "admin")]
    public class NoticesModel(INoticeRepository noticeRepository, IHubContext<NoticeHub> hub) : BasePageModel
    {
        [BindProperty]
        public NoticeInput Input { get; set; } = new();

        public List<Notice> Notices { get; set; } = new();

        public async Task OnGetAsync()
        {
            Notices = await noticeRepository.GetAllAsync();

            Input.ScheduledStart = DateTime.Now.AddMinutes(30);
            Input.ScheduledEnd = DateTime.Now.AddMinutes(60);
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            if (Input.ScheduledEnd <= Input.ScheduledStart)
            {
                ModelState.AddModelError("Input.ScheduledEnd", "The expected end must be after the start.");
            }

            if (!ModelState.IsValid)
            {
                Notices = await noticeRepository.GetAllAsync();
                return Page();
            }

            var notice = new Notice
            {
                Title = Input.Title,
                Message = Input.Message,
                ScheduledStart = DateTime.SpecifyKind(Input.ScheduledStart, DateTimeKind.Local).ToUniversalTime(),
                ScheduledEnd = DateTime.SpecifyKind(Input.ScheduledEnd, DateTimeKind.Local).ToUniversalTime(),
                IsPublished = Input.PublishImmediately,
                IsActive = true
            };

            await noticeRepository.CreateAsync(notice);
            if (notice.IsPublished)
            {
                await hub.Clients.All.SendAsync("MaintenanceUpdated", notice);
            }

            TempData["SuccessMessage"] = "Maintenance announcement created.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            await noticeRepository.CancelAsync(id);
            await hub.Clients.All.SendAsync("MaintenanceCancelled", id);

            TempData["SuccessMessage"] = "Maintenance announcement cancelled.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPublishAsync(int id)
        {
            var notice = await noticeRepository.PublishAsync(id);
            await hub.Clients.All.SendAsync("MaintenanceUpdated", notice);

            TempData["SuccessMessage"] = "Maintenance announcement published.";
            return RedirectToPage();
        }
    }
}
