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
    public class AddModel(IDarklandsCardRepository repository, ILogEntryRepository logEntryRepository, Config config) : BasePageModel
    {
        [BindProperty]
        public DarklandsCard AddCard { get; set; } = new();

        [BindProperty]
        public IFormFile Image { get; set; }

        public IList<DarklandsCardType> CardTypes { get; set; } = [];
        public IList<DarklandsOverlord> Overlords { get; set; } = [];

        public async Task OnGetAsync()
        {
            await LoadOptions();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Image == null || Image.Length == 0)
            {
                ModelState.AddModelError(nameof(Image), "Please select an image.");
            }

            if (!ModelState.IsValid)
            {
                await LoadOptions();
                return Page();
            }

            var loggedInUser = await GetUser();

            try
            {
                // Resolve the related entities from the database.
                AddCard.CardType = (await repository.GetDarklandsCardTypes())
                    .FirstOrDefault(x => x.Id == AddCard.CardTypeId)!;

                // Save the original uploaded image.
                var fileName = $"{AddCard.Name.Replace(" ", "_")}{Path.GetExtension(Image!.FileName)}";
                var uploadPath = Path.Combine(config.AppData.UploadPath, fileName);

                Directory.CreateDirectory(config.AppData.UploadPath);

                await using (var stream = System.IO.File.Create(uploadPath))
                {
                    await Image.CopyToAsync(stream);
                }

                AddCard.Images = [];
                AddCard.Overlord = null;
                AddCard.CardType = null;

                var addedCard = await repository.AddDarklandsCard(AddCard) ?? throw new InvalidOperationException(
                        $"Error inserting Darklands card {AddCard.Name}.");

                // Original image.
                await repository.AddDarklandsCardImage(new DarklandsCardImage
                {
                    CardId = addedCard.Id,
                    CardImageTypeId = 2,
                    ImageUrl = $"upload/{fileName}"
                });

                // Small image.
                var smallImagePath = await CreateSmallImage(
                    AddCard.Name, Image, 227, 320, "small");

                await repository.AddDarklandsCardImage(new DarklandsCardImage
                {
                    CardId = addedCard.Id,
                    CardImageTypeId = 1,
                    ImageUrl = smallImagePath
                });

                // Medium image.
                var mediumImagePath = await CreateSmallImage(
                    AddCard.Name, Image, 600, 850, "medium");

                await repository.AddDarklandsCardImage(new DarklandsCardImage
                {
                    CardId = addedCard.Id,
                    CardImageTypeId = 4,
                    ImageUrl = mediumImagePath
                });
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

                ModelState.AddModelError(
                    string.Empty,
                    "There was an error adding the card. Check the error logs.");

                await LoadOptions();
                return Page();
            }

            return RedirectToPage("./Manage");
        }

        private async Task LoadOptions()
        {
            CardTypes = await repository.GetDarklandsCardTypes();
            Overlords = await repository.GetDarklandsOverlords();
        }

        private async Task<string> CreateSmallImage(
            string cardName,
            IFormFile file,
            int maxWidth,
            int maxHeight,
            string size)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty or null.");

            if (maxWidth <= 0 || maxHeight <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxWidth));

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

            var uniqueFileName = $"{cardName.Replace(" ", "_")}.png";
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
    }
}