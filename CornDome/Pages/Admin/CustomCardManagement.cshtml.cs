using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class CustomCardManagementModel(ICustomCardRepository customCardRepository, IConfiguration configuration, ILogger<IndexModel> logger) : BasePageModel
    {
        public List<CustomCard> Cards { get; set; } = new();

        public string Filter { get; set; } = "all";
        public string Search { get; set; } = string.Empty;

        public int TotalCount => Cards.Count;
        public int PendingCount => Cards.Count(x => x.ApprovedBy == null);
        public int ApprovedCount => Cards.Count(x => x.ApprovedBy > 0);
        public int RejectedCount => Cards.Count(x => x.ApprovedBy == 0);

        public async Task OnGetAsync(string filter = "all", string? search = null)
        {
            Filter = filter;
            Search = search ?? string.Empty;

            Cards = await customCardRepository.GetAll();

            Cards = Filter switch
            {
                "pending" => Cards.Where(x => x.ApprovedBy == null).ToList(),
                "approved" => Cards.Where(x => x.ApprovedBy > 0).ToList(),
                "rejected" => Cards.Where(x => x.ApprovedBy == 0).ToList(),
                _ => Cards
            };

            if (!string.IsNullOrWhiteSpace(Search))
            {
                Cards = Cards.Where(x =>
                    x.Name.Contains(Search, StringComparison.OrdinalIgnoreCase) ||
                    (x.Author?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.Artist?.Contains(Search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    x.UserId.ToString() == Search).ToList();
            }
        }

        public async Task<IActionResult> OnPostApproveAsync(Guid id)
        {
            var userId = GetAdminUserId();

            if (userId == null)
                return Unauthorized();

            if (!await customCardRepository.Approve(id, userId.Value))
                return NotFound();

            TempData["Success"] = "Card approved successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid id)
        {
            if (!await customCardRepository.Deny(id))
                return NotFound();

            TempData["Success"] = "Card rejected.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetApprovalAsync(Guid id)
        {
            var card = await customCardRepository.GetById(id);

            if (card == null)
                return NotFound();

            card.ApprovedBy = null;

            if (!await customCardRepository.Update(card))
                return NotFound();

            TempData["Success"] = "Card returned to pending approval.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            var card = await customCardRepository.GetById(id);

            if (card == null)
                return NotFound();

            var imageDirectory = configuration.GetValue<string>("Cards:Custom");

            if (string.IsNullOrWhiteSpace(imageDirectory))
                imageDirectory = null;

            if (!await customCardRepository.Delete(id))
                return NotFound();

            if (imageDirectory != null && !string.IsNullOrWhiteSpace(card.ImageUrl))
            {
                var fileName = $"{id:N}.png";
                var path = Path.Combine(imageDirectory, fileName);

                try
                {
                    if (System.IO.File.Exists(path))
                        System.IO.File.Delete(path);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Unable to delete custom card image {CardId}", id);
                }
            }

            TempData["Success"] = "Card deleted.";
            return RedirectToPage();
        }

        private int? GetAdminUserId()
        {
            var value = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
