using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

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
        public string BaseUrl { get; set; } = config.BaseUrl;

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
                    var isEverythingGoingWell = false;
                    var cardRevision = card.Revisions.FirstOrDefault(x => x.Id == imageToUpload.RevisionId);

                    if (cardRevision == null)
                        continue;

                    string updatedString = await UploadImage(cardRevision, imageToUpload);
                    if (!string.IsNullOrEmpty(updatedString))
                    {
                        isEverythingGoingWell = cardRepository.UpdateRevisionImage(cardRevision, updatedString, 2);
                    }

                    // Add a small image if we updated successfully
                    if (isEverythingGoingWell)
                    {
                        var smallImagePath = await CreateSmallImage(cardRevision, imageToUpload.File, 227, 320);
                        if (!string.IsNullOrEmpty(smallImagePath))
                        {
                            CardImage smallImage = new()
                            {
                                CardImageTypeId = (int)CardImageTypeEnum.Small,
                                ImageUrl = smallImagePath,
                                RevisionId = imageToUpload.RevisionId
                            };
                            isEverythingGoingWell = cardRepository.UpdateRevisionImage(cardRevision, smallImagePath, 1);
                        }
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

        private async Task<string> CreateSmallImage(CardRevision cardRevision, IFormFile file, int maxWidth, int maxHeight)
        {
            var uploadsFolder = config.AppData.ImagePath;
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            await using var inputStream = file.OpenReadStream();
            using var image = await SixLabors.ImageSharp.Image.LoadAsync(inputStream);

            // Resize with high quality
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max, // Keeps aspect ratio
                Size = new Size(maxWidth, maxHeight),
                Sampler = KnownResamplers.Lanczos3 // Very good quality
            }));

            // Save with a unique filename
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{cardRevision.Name.Replace(" ", "_")}{ext}";
            var filePath = Path.Combine(uploadsFolder, "generated", "small", uniqueFileName);
            await using var outputStream = new FileStream(filePath, FileMode.OpenOrCreate);
            await image.SaveAsync(outputStream, new PngEncoder()); // or new JpegEncoder()

            return $"generated/small/{uniqueFileName}";
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

            var outputFileName = $"upload/{uniqueFileName}";

            return outputFileName;
        }
    }

    public class ImageUpload
    {
        public IFormFile File { get; set; }
        public int RevisionId { get; set; }
    }
}
