using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Pages.CardCreator
{
    [Authorize]
    public class MineModel(ICustomCardRepository customCardRepository, Config configuration, ILogger<MineModel> logger) : BasePageModel
    {
        public List<CustomCard> Cards { get; set; } = new();
        public int FreeCustomCardLimit = 10;
        public int UploadCount { get; set; } = 0;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            Cards = await customCardRepository.GetByUserId(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateVisibilityAsync(Guid id, Visibility visibility)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            if (!Enum.IsDefined(typeof(Visibility), visibility))
                return BadRequest(new { success = false, message = "Invalid visibility." });

            var updated = await customCardRepository.UpdateVisibility(id, userId, visibility);

            if (!updated)
                return NotFound(new { success = false, message = "Card not found." });

            return new JsonResult(new { success = true, message = "Visibility updated." });
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            if (!TryGetUserId(out var userId))
                return Unauthorized();

            var card = await customCardRepository.GetById(id);

            if (card == null || card.UserId != userId)
                return NotFound(new { success = false, message = "Card not found." });

            var deleted = await customCardRepository.Delete(id, userId);

            if (!deleted)
                return NotFound(new { success = false, message = "Card not found." });

            try
            {
                var imageDirectory = configuration.AppData.CustomPath;

                if (!string.IsNullOrWhiteSpace(imageDirectory))
                {
                    var imagePath = Path.Combine(imageDirectory, $"{id:N}.png");

                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unable to delete image for custom card {CardId}", id);
            }

            return new JsonResult(new { success = true, message = "Card deleted." });
        }

        private bool TryGetUserId(out int userId)
        {
            return int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
        }
    }
}