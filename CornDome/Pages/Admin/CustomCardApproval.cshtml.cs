using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "cardManager")]
    public class CustomCardApprovalModel(ICustomCardRepository customCardRepository) : BasePageModel
    {
        public List<CustomCard> Cards { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            Cards = await customCardRepository.GetUnapproved();
            return Page();
        }

        public async Task<IActionResult> OnPostApproveAsync(Guid id)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var adminUserId))
                return Unauthorized();

            var approved = await customCardRepository.Approve(id, adminUserId);

            if (!approved)
                return NotFound(new { success = false, message = "Card not found or already approved." });

            return new JsonResult(new
            {
                success = true,
                message = "Card approved successfully."
            });
        }

        public async Task<IActionResult> OnPostDenyAsync(Guid id)
        {
            var denied = await customCardRepository.Deny(id);

            if (!denied)
                return NotFound(new { success = false, message = "Card not found or already processed." });

            return new JsonResult(new { success = true, message = "Card denied." });
        }
    }
}
