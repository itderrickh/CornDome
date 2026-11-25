using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Models.Users;
using CornDome.Repository.Cards;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System.Security.Claims;

namespace CornDome.Pages.CardCreator
{
    public class SaveCustomCardRequest
    {
        public string Name { get; set; } = string.Empty;
        public int CardType { get; set; }
        public int? Landscape { get; set; }
        public string Ability { get; set; } = string.Empty;

        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }

        public string? Author { get; set; }
        public string? Artist { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Invisible;

        public string Image { get; set; } = string.Empty;
    }

    public class IndexModel(ICustomCardRepository customCardRepository, Config configuration, ILogger<IndexModel> logger, UserRoleHelper userRoleHelper) : BasePageModel
    {
        private const int MaxImageBytes = 12 * 1024 * 1024;
        private const int MaxImageWidth = 4096;
        private const int MaxImageHeight = 4096;
        private const long MaxImagePixels = 12_000_000;
        public int FreeCustomCardLimit = 10;
        public int UploadCount { get; set; } = 0;

        public async Task OnGet()
        {
            if (User != null && User.Identity.IsAuthenticated)
            {
                var user = await GetUser();
                UploadCount = await customCardRepository.CountByUserId(user.Id);
            }
        }

        public async Task<IActionResult> OnPostSaveAsync([FromBody] SaveCustomCardRequest request)
        {
            if (User.Identity?.IsAuthenticated != true)
                return Unauthorized();

            var user = await GetUser();

            if (!userRoleHelper.IsPatreonMember(User) && !userRoleHelper.IsAdmin(User))
            {
                var existingCards = await customCardRepository.CountByUserId(user.Id);

                if (existingCards >= FreeCustomCardLimit)
                {
                    return StatusCode(403, new
                    {
                        success = false,
                        message = "You've reached the limit of 10 custom cards. Become a Patreon supporter to create more!"
                    });
                }
            }

            if (request == null)
                return BadRequest(new { success = false, message = "Invalid card data." });

            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out var userId))
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Length > 60)
                return BadRequest(new { success = false, message = "Card name must be between 1 and 60 characters." });

            if (request.CardType > 0)
                return BadRequest(new { success = false, message = "Invalid card type." });

            if (request.Ability?.Length > 1000 || request.Author?.Length > 60 || request.Artist?.Length > 60)
                return BadRequest(new { success = false, message = "One or more card fields are too long." });

            if (new[] { request.Cost, request.Attack, request.Defense }.Any(x => x.HasValue && (x.Value < 0 || x.Value > 99)))
                return BadRequest(new { success = false, message = "Card values must be between 0 and 99." });

            if (!request.Image.StartsWith("data:image/png;base64,", StringComparison.Ordinal))
                return BadRequest(new { success = false, message = "The card image must be a PNG." });

            if (!Enum.IsDefined(typeof(Visibility), request.Visibility))
                return BadRequest(new { success = false, message = "Invalid visibility option." });

            byte[] imageBytes;

            try
            {
                imageBytes = Convert.FromBase64String(request.Image["data:image/png;base64,".Length..]);
            }
            catch (FormatException)
            {
                return BadRequest(new { success = false, message = "Invalid image data." });
            }

            if (imageBytes.Length == 0 || imageBytes.Length > MaxImageBytes)
                return BadRequest(new { success = false, message = "The generated image is too large." });

            using var imageData = SKData.CreateCopy(imageBytes);
            using var imageInfo = SKCodec.Create(imageData);

            if (imageInfo == null)
                return BadRequest(new { success = false, message = "Unable to read the generated image." });

            using (imageInfo)
            {
                var imageWidth = imageInfo.Info.Width;
                var imageHeight = imageInfo.Info.Height;

                if (imageWidth <= 0 || imageHeight <= 0 ||
                    imageWidth > MaxImageWidth || imageHeight > MaxImageHeight ||
                    (long)imageWidth * imageHeight > MaxImagePixels)
                    return BadRequest(new { success = false, message = "Image dimensions are too large." });
            }

            using var bitmap = SKBitmap.Decode(imageBytes);

            if (bitmap == null)
                return BadRequest(new { success = false, message = "Unable to decode the generated image." });

            using var image = SKImage.FromBitmap(bitmap);
            using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);

            if (encoded == null)
                return BadRequest(new { success = false, message = "Unable to process the generated image." });

            var imageDirectory = configuration.AppData.CustomPath;

            if (string.IsNullOrWhiteSpace(imageDirectory))
                throw new InvalidOperationException("Cards:Custom is not configured.");

            Directory.CreateDirectory(imageDirectory);

            var id = Guid.NewGuid();
            var fileName = $"{id:N}.png";
            var imagePath = Path.Combine(imageDirectory, fileName);

            try
            {
                await using (var stream = System.IO.File.Create(imagePath))
                {
                    encoded.SaveTo(stream);
                }

                var card = new CustomCard
                {
                    Id = id,
                    UserId = userId,
                    Name = request.Name.Trim(),
                    CardType = request.CardType,
                    Landscape = request.Landscape,
                    Ability = request.Ability?.Trim() ?? string.Empty,
                    Cost = request.Cost,
                    Attack = request.Attack,
                    Defense = request.Defense,
                    Author = string.IsNullOrWhiteSpace(request.Author) ? null : request.Author.Trim(),
                    Artist = string.IsNullOrWhiteSpace(request.Artist) ? null : request.Artist.Trim(),
                    ImageUrl = $"/api/customcard/{id}/image",
                    CreatedAt = DateTime.UtcNow,
                    Visibility = request.Visibility,
                    ApprovedBy = null
                };

                var dbCard = await customCardRepository.Add(card);

                return new JsonResult(new
                {
                    success = true,
                    id = dbCard.Id,
                    message = "Your custom card was saved."
                });
            }
            catch (Exception ex)
            {
                if (System.IO.File.Exists(imagePath))
                    System.IO.File.Delete(imagePath);

                logger.LogError(ex, "Error saving custom card for user {UserId}", userId);
                return StatusCode(500, new { success = false, message = "Unable to save your card." });
            }
        }
    }
}
