using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;

namespace CornDome.Pages.CardManage
{
    [Authorize(Policy = "cardManager")]
    public class AddModel(ICardRepository cardRepository, ILogEntryRepository logEntryRepository, Config config) : BasePageModel
    {
        [BindProperty]
        public AddCard AddCard { get; set; } = new();

        public List<CardSet> AllSets { get; set; }

        public void OnGet()
        {
            AllSets = cardRepository.GetAllSets();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var loggedInUser = await GetUser();

            var setIds = AddCard.SetIds.Split(',').Select(int.Parse).ToArray();
            var sets = cardRepository.GetSetsByIds(setIds);
            var card = new Card() { IsCustomCard = AddCard.IsCustomCard };
            var revision = new CardRevision()
            {
                Ability = AddCard.Ability,
                Attack = AddCard.Attack,
                Defense = AddCard.Defense,
                CardSets = sets,
                TypeId = AddCard.TypeId,
                LandscapeId = AddCard.LandscapeId,
                Cost = AddCard.Cost,
                Name = AddCard.Name,
                RevisionNumber = 1
            };
            var cardImage = new CardImage()
            {
                CardImageTypeId = 2,
                ImageUrl = $"upload/{AddCard.Name.Replace(" ", "_")}{Path.GetExtension(AddCard.Image.FileName)}"
            };

            using (var stream = System.IO.File.Create($"{config.AppData.UploadPath}/{AddCard.Name.Replace(" ", "_")}{Path.GetExtension(AddCard.Image.FileName)}"))
            {
                await AddCard.Image.CopyToAsync(stream);
            }

            var addedCard = cardRepository.AddCard(card, revision);

            if (addedCard != null)
            {
                cardImage.RevisionId = addedCard.LatestRevision.Id;
                cardRepository.AddRevisionImage(cardImage);
            }
            else
            {
                await logEntryRepository.InsertAsync(new LogEntry()
                {
                    Timestamp = DateTime.Now,
                    Exception = $"Error inserting card {AddCard?.Name}",
                    Method = "OnPost",
                    UserName = loggedInUser?.UserName
                });
            }

            var smallImagePath = await CreateSmallImage(revision, AddCard.Image, 227, 320, "small");
            if (smallImagePath != null && addedCard != null)
            {
                var cardImageSmall = new CardImage()
                {
                    CardImageTypeId = 1,
                    ImageUrl = smallImagePath,
                    RevisionId = addedCard.LatestRevision.Id
                };
                cardRepository.AddRevisionImage(cardImageSmall);
            }
            else
            {
                await logEntryRepository.InsertAsync(new LogEntry()
                {
                    Timestamp = DateTime.Now,
                    Exception = $"Error inserting card {AddCard?.Name}",
                    Method = "OnPost",
                    UserName = loggedInUser?.UserName
                });
            }

            var optimizedImagePath = await CreateSmallImage(revision, AddCard.Image, 600, 850, "medium");
            if (optimizedImagePath != null && addedCard != null)
            {
                var cardImageOptimized = new CardImage()
                {
                    CardImageTypeId = 4,
                    ImageUrl = optimizedImagePath,
                    RevisionId = addedCard.LatestRevision.Id
                };
                cardRepository.AddRevisionImage(cardImageOptimized);
            }
            else
            {
                await logEntryRepository.InsertAsync(new LogEntry()
                {
                    Timestamp = DateTime.Now,
                    Exception = $"Error inserting card {AddCard?.Name}",
                    Method = "OnPost",
                    UserName = loggedInUser?.UserName
                });
            }

            return RedirectToPage("/CardManage/Index");
        }


        private async Task<string> CreateSmallImage(CardRevision cardRevision, IFormFile file, int maxWidth, int maxHeight, string size)
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
    }


    public class AddCard
    {
        public bool IsCustomCard { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Ability { get; set; }
        public string SetIds { get; set; }
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
