using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class EditModel(ICardRepository cardRepository, Config config) : BasePageModel
    {
        [BindProperty(Name = "id", SupportsGet = true)]
        public int CardId { get; set; }
        [BindProperty]
        public EditCard EditCard { get; set; }
        [BindProperty]
        public ImageUpload RevisionImageUpload { get; set; }
        public List<CardSet> AllSets { get; set; }
        [BindProperty]
        public string SetIds { get; set; }

        public void OnGet()
        {
            AllSets = cardRepository.GetAllSets();
            var cardFound = cardRepository.GetCard(CardId);
            EditCard = new EditCard()
            {
                IsCustomCard = cardFound.IsCustomCard,
                Name = cardFound.Name,
                TypeId = cardFound.TypeId,
                Ability = cardFound.Ability,
                LandscapeId = cardFound.LandscapeId,
                Cost = cardFound.Cost,
                Attack = cardFound.Attack,
                Defense = cardFound.Defense,
                Id = cardFound.Id,
                CardSets = [.. cardFound.CardSets],
                ImageUrl = cardFound.GetOptimizedImage
            };
        }

        public async Task<IActionResult> OnPost()
        {
            var isSuccess = false;
            var card = cardRepository.GetCard(CardId);

            var imageToUpload = RevisionImageUpload;
            if (imageToUpload.File != null)
            {
                var isEverythingGoingWell = false;

                string updatedString = await UploadImage(card, imageToUpload);
                if (!string.IsNullOrEmpty(updatedString))
                {
                    isEverythingGoingWell = cardRepository.UpdateCardImage(card.Id, updatedString, 2);
                }

                // Add a small image if we updated successfully
                if (isEverythingGoingWell)
                {
                    var smallImagePath = await CreateSmallImage(card, imageToUpload.File, 227, 320, "small");
                    if (!string.IsNullOrEmpty(smallImagePath))
                    {
                        CardImage smallImage = new()
                        {
                            CardImageTypeId = (int)CardImageTypeEnum.Small,
                            ImageUrl = smallImagePath
                        };
                        isEverythingGoingWell = cardRepository.UpdateCardImage(card.Id, smallImagePath, 1);
                    }
                }

                // Add a optimized image if we updated successfully
                if (isEverythingGoingWell)
                {
                    var optimizedImagePath = await CreateSmallImage(card, imageToUpload.File, 600, 850, "medium");
                    if (optimizedImagePath != null)
                    {
                        var cardImageOptimized = new CardImage()
                        {
                            CardImageTypeId = 4,
                            ImageUrl = optimizedImagePath
                        };
                        isEverythingGoingWell = cardRepository.UpdateCardImage(card.Id, optimizedImagePath, 4);
                    }
                }
            }

            if (card != null && card.Id == CardId)
            {
                var setsToUpdate = new List<CardSet>();

                if (SetIds != null)
                {
                    var setIds = SetIds.Split(',').Select(int.Parse).ToArray();
                    setsToUpdate = cardRepository.GetSetsByIds(setIds);
                }

                card.CardSets.Clear();

                card.Ability = EditCard.Ability;
                card.Attack = EditCard.Attack;
                card.IsCustomCard = EditCard.IsCustomCard;
                card.TypeId = EditCard.TypeId;
                card.Name = EditCard.Name;
                card.Cost = EditCard.Cost;
                card.Defense = EditCard.Defense;
                card.LandscapeId = EditCard.LandscapeId;
                card.CardSets = setsToUpdate;

                isSuccess = cardRepository.UpdateCard(card);
            }

            if (isSuccess)
                TempData["SuccessMessage"] = "Card saved successfully.";
            else
                TempData["ErrorMessage"] = "There was an issue saving the card.";

            EditCard = new EditCard()
            {
                IsCustomCard = card.IsCustomCard,
                Name = card.Name,
                TypeId = card.TypeId,
                Ability = card.Ability,
                LandscapeId = card.LandscapeId,
                Cost = card.Cost,
                Attack = card.Attack,
                Defense = card.Defense,
                Id = card.Id,
                ImageUrl = card.GetOptimizedImage,
                CardSets = [.. card.CardSets]
            };
            return RedirectToPage(new { id = CardId });
        }

        private async Task<string> CreateSmallImage(Card cardRevision, IFormFile file, int maxWidth, int maxHeight, string size)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null");

            if (maxWidth <= 0 || maxHeight <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(maxWidth), "Image dimensions must be positive.");

            var uploadsFolder = config.AppData.ImagePath;
            var outputFolder = Path.Combine(
                uploadsFolder, "generated", size);

            Directory.CreateDirectory(outputFolder);

            await using var inputStream = file.OpenReadStream();
            using var bitmap = SKBitmap.Decode(inputStream);

            if (bitmap == null)
                throw new ArgumentException("Invalid or unsupported image.");

            // Calculate dimensions while preserving the aspect ratio.
            var scale = Math.Min(
                (double)maxWidth / bitmap.Width,
                (double)maxHeight / bitmap.Height);

            // Don't enlarge images that are already smaller.
            scale = Math.Min(scale, 1.0);

            var width = Math.Max(1, (int)Math.Round(bitmap.Width * scale));
            var height = Math.Max(1, (int)Math.Round(bitmap.Height * scale));

            using var resized = bitmap.Resize(
                new SKImageInfo(width, height),
                new SKSamplingOptions(SKFilterMode.Linear, SKMipmapMode.None))
                ?? throw new InvalidOperationException("Image resize failed.");

            using var image = SKImage.FromBitmap(resized);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);

            var uniqueFileName =
                $"{cardRevision.Name.Replace(" ", "_")}.png";

            var filePath = Path.Combine(outputFolder, uniqueFileName);

            await using (var outputStream = new FileStream(
                filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                encoded.SaveTo(outputStream);
            }

            return $"generated/{size}/{uniqueFileName}";
        }

        private async Task<string> UploadImage(Card cardRevision, ImageUpload upload)
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
    }

    public class EditCard
    {
        public bool IsCustomCard { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Ability { get; set; }
        public List<CardSet> CardSets { get; set; }
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
        public int Id { get; set; }
    }
}
