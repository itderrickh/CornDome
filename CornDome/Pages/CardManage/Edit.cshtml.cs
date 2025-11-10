using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class EditModel(ICardRepository cardRepository, Config config) : PageModel
    {
        [BindProperty]
        public int CardId { get; set; }
        [BindProperty]
        public Card EditCard { get; set; }
        [BindProperty]
        public List<ImageUpload> RevisionImageUploads { get; set; } = [];

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            CardId = int.Parse(queryId);

            EditCard = cardRepository.GetCard(CardId);
        }

        public async Task<IActionResult> OnPost()
        {
            var isSuccess = false;
            var card = cardRepository.GetCard(CardId);

            foreach (var imageToUpload in RevisionImageUploads)
            {
                if (imageToUpload.File != null)
                {
                    var cardRevision = card.Revisions.FirstOrDefault(x => x.Id == imageToUpload.RevisionId);

                    if (cardRevision == null)
                        continue;

                    string updatedString = await UploadImage(cardRevision, imageToUpload);
                    if (!string.IsNullOrEmpty(updatedString))
                    {
                        cardRepository.UpdateRevisionImage(cardRevision, updatedString);
                    }
                }
            }

            if (card != null && card.Id == CardId)
            {
                isSuccess = cardRepository.UpdateCardAndRevisions(EditCard);
                isSuccess = true;
            }

            if (isSuccess)
                TempData["SuccessMessage"] = "Card saved successfully.";
            else
                TempData["ErrorMessage"] = "There was an issue saving the card.";

            EditCard = cardRepository.GetCard(CardId);
            return RedirectToPage(new { id = CardId });
        }

        private async Task<string> UploadImage(CardRevision cardRevision, ImageUpload upload)
        {
            var uploadsFolder = config.AppData.UploadPath;
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(upload.File.FileName).ToLowerInvariant();
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            if (!permittedExtensions.Contains(ext))
                return null;

            // Save with a unique filename
            var uniqueFileName = $"{cardRevision.Name.Replace(" ", "_")}{ext}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await upload.File.CopyToAsync(stream);
            }

            return $"upload/{uniqueFileName}";
        }
    }

    public class ImageUpload
    {
        public IFormFile File { get; set; }
        public int RevisionId { get; set; }
    }
}
