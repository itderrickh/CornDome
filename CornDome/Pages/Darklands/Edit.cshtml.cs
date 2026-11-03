using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace CornDome.Pages.Darklands
{
    [Authorize(Policy = "cardManager")]
    public class EditModel(IDarklandsCardRepository repository, ILogEntryRepository logEntryRepository, Config config) : BasePageModel
    {
        [BindProperty]
        public int CardId { get; set; }

        [BindProperty]
        public DarklandsCard EditCard { get; set; } = new();

        [BindProperty]
        public IFormFile? Image { get; set; }

        public IList<DarklandsCardType> CardTypes { get; set; } = [];
        public IList<DarklandsOverlord> Overlords { get; set; } = [];

        public string? OldImageUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            CardId = id;

            var card = await repository.GetDarklandsCard(id);

            if (card == null)
                return NotFound();

            EditCard = card;
            OldImageUrl = GetOriginalImage(card);

            await LoadOptions();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await ReloadPageData();
                return Page();
            }

            var loggedInUser = await GetUser();

            try
            {
                var existingCard = await repository.GetDarklandsCard(CardId);

                if (existingCard == null)
                    return NotFound();

                // Validate the selected related entities.
                var cardType = (await repository.GetDarklandsCardTypes())
                    .FirstOrDefault(x => x.Id == EditCard.CardTypeId);

                if (cardType == null)
                {
                    ModelState.AddModelError(
                        "EditCard.CardTypeId",
                        "Please select a valid card type.");

                    await ReloadPageData();
                    return Page();
                }

                DarklandsOverlord? overlord = null;

                if (EditCard.OverlordId.HasValue)
                {
                    overlord = (await repository.GetDarklandsOverlords())
                        .FirstOrDefault(x => x.Id == EditCard.OverlordId.Value);

                    if (overlord == null)
                    {
                        ModelState.AddModelError(
                            "EditCard.OverlordId",
                            "Please select a valid Overlord.");

                        await ReloadPageData();
                        return Page();
                    }
                }

                // Update card properties without replacing its navigation collections.
                existingCard.Name = EditCard.Name;
                existingCard.CardTypeId = EditCard.CardTypeId;
                existingCard.OverlordId = EditCard.OverlordId;
                existingCard.Ability = EditCard.Ability;
                existingCard.Attack = EditCard.Attack;
                existingCard.Defense = EditCard.Defense;

                existingCard.CardType = cardType;
                existingCard.Overlord = overlord;

                await repository.EditDarklandsCard(existingCard);

                // Only replace images when a new image was uploaded.
                if (Image != null && Image.Length > 0)
                {
                    await ReplaceImages(existingCard);
                }

                TempData["SuccessMessage"] = "Darklands card updated successfully.";

                return RedirectToPage("./Edit", new { id = CardId });
            }
            catch (Exception ex)
            {
                await logEntryRepository.InsertAsync(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Exception = ex.ToString(),
                    Method = "OnPost",
                    UserName = loggedInUser?.UserName
                });

                TempData["ErrorMessage"] = "There was an error updating the card.";

                await ReloadPageData();
                return Page();
            }
        }

        private async Task ReplaceImages(DarklandsCard card)
        {
            var fileName = $"{Guid.NewGuid():N}{Path.GetExtension(Image!.FileName)}";

            Directory.CreateDirectory(config.AppData.UploadPath);

            var uploadPath = Path.Combine(config.AppData.UploadPath, fileName);

            await using (var stream = System.IO.File.Create(uploadPath))
            {
                await Image.CopyToAsync(stream);
            }

            // Remove the existing image records before adding replacements.
            await repository.DeleteDarklandsCardImages(card.Id);

            // Original image.
            await repository.AddDarklandsCardImage(new DarklandsCardImage
            {
                CardId = card.Id,
                CardImageTypeId = 2,
                ImageUrl = $"upload/{fileName}"
            });

            // Small image.
            var smallImagePath = await CreateSmallImage(
                card.Name, Image, 227, 320, "small");

            await repository.AddDarklandsCardImage(new DarklandsCardImage
            {
                CardId = card.Id,
                CardImageTypeId = 1,
                ImageUrl = smallImagePath
            });

            // Medium image.
            var mediumImagePath = await CreateSmallImage(
                card.Name, Image, 600, 850, "medium");

            await repository.AddDarklandsCardImage(new DarklandsCardImage
            {
                CardId = card.Id,
                CardImageTypeId = 4,
                ImageUrl = mediumImagePath
            });
        }

        private async Task<string> CreateSmallImage(
            string cardName,
            IFormFile file,
            int maxWidth,
            int maxHeight,
            string size)
        {
            if (file.Length == 0)
                throw new ArgumentException("File is empty.");

            var outputFolder = Path.Combine(
                config.AppData.ImagePath,
                "generated",
                size);

            Directory.CreateDirectory(outputFolder);

            await using var inputStream = file.OpenReadStream();
            using var bitmap = SKBitmap.Decode(inputStream);

            if (bitmap == null)
                throw new ArgumentException("Invalid or unsupported image.");

            var scale = Math.Min(
                (double)maxWidth / bitmap.Width,
                (double)maxHeight / bitmap.Height);

            scale = Math.Min(scale, 1.0);

            var width = Math.Max(1, (int)Math.Round(bitmap.Width * scale));
            var height = Math.Max(1, (int)Math.Round(bitmap.Height * scale));

            using var resized = bitmap.Resize(
                new SKImageInfo(width, height),
                new SKSamplingOptions(
                    SKFilterMode.Linear,
                    SKMipmapMode.None))
                ?? throw new InvalidOperationException("Image resize failed.");

            using var image = SKImage.FromBitmap(resized);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);

            var uniqueFileName = $"{Guid.NewGuid():N}.png";
            var filePath = Path.Combine(outputFolder, uniqueFileName);

            await using (var outputStream = new FileStream(
                filePath,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None))
            {
                encoded.SaveTo(outputStream);
            }

            return $"generated/{size}/{uniqueFileName}";
        }

        private string? GetOriginalImage(DarklandsCard card)
        {
            return card.Images?
                .FirstOrDefault(x => x.CardImageTypeId == 2)?.ImageUrl
                ?? card.Images?.FirstOrDefault()?.ImageUrl;
        }

        private async Task LoadOptions()
        {
            CardTypes = await repository.GetDarklandsCardTypes();
            Overlords = await repository.GetDarklandsOverlords();
        }

        private async Task ReloadPageData()
        {
            var card = await repository.GetDarklandsCard(CardId);

            if (card != null)
            {
                OldImageUrl = GetOriginalImage(card);
            }

            await LoadOptions();
        }
    }
}