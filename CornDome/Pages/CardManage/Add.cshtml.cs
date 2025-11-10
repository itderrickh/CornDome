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
    public class AddModel(ICardRepository cardRepository, Config config) : PageModel
    {
        [BindProperty]
        public AddCard AddCard { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var card = new Card() { IsCustomCard = AddCard.IsCustomCard };
            var revision = new CardRevision()
            {
                Ability = AddCard.Ability,
                Attack = AddCard.Attack,
                Defense = AddCard.Defense,
                SetId = AddCard.SetId,
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

            var addedCard = cardRepository.AddCard(card, revision, cardImage);

            var smallImagePath = await CreateSmallImage(revision, AddCard.Image, 27, 320);
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

            return RedirectToPage("/CardManage/Index");
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

            // Calculate resize ratio while preserving aspect ratio
            double ratioX = (double)maxWidth / image.Width;
            double ratioY = (double)maxHeight / image.Height;
            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(image.Width * ratio);
            int newHeight = (int)(image.Height * ratio);

            // Resize with high quality
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Mode = ResizeMode.Max, // Keeps aspect ratio
                Size = new Size(newWidth, newHeight),
                Sampler = KnownResamplers.Lanczos3 // Very good quality
            }));

            await using var outputStream = new MemoryStream();
            await image.SaveAsync(outputStream, new PngEncoder()); // or new JpegEncoder()

            // Save with a unique filename
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var uniqueFileName = $"{cardRevision.Name.Replace(" ", "_")}{ext}";
            var filePath = Path.Combine(uploadsFolder, "generated", "small", uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(stream);
            }

            return $"generated/small/{uniqueFileName}";
        }
    }

    public class AddCard
    {
        public bool IsCustomCard { get; set; }
        public string Name { get; set; }
        public int TypeId { get; set; }
        public string Ability { get; set; }
        public int? SetId { get; set; }
        public int? LandscapeId { get; set; }
        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }
    }
}
