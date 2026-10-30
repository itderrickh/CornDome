using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Models.Users;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using System.Collections.Concurrent;
using System.Web;

namespace CornDome.Pages
{
    public class DownloadModel(Config configuration, ICardRepository cardRepository, IDeckRepository deckRepository) : BasePageModel
    {
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        private readonly Config config = configuration;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;

        private const int cardWidth = 86;
        private const int cardHeight = 120;
        private const int borderX = 20;
        private const int borderY = 20;

        private const int finalImageWidth = (borderX * 2) + (cardWidth * 10);

        private int finalImageHeight;

        public string ImageString { get; set; }

        public ConcurrentDictionary<string, SKBitmap> imageCache = new();

        public bool IsInvalid = false;

        public byte[] CreateCoordinates()
        {
            var coordinates = new List<(int X, int Y)>();

            // Hero and landscape positions.
            for (var topX = 0; topX < 5; topX++)
            {
                coordinates.Add((
                    borderX + (topX * cardWidth * 2),
                    borderY));
            }

            var numberOfRows = (int)Math.Round(
                QueryDeck.Cards.Count / 10m,
                MidpointRounding.ToPositiveInfinity);

            finalImageHeight =
                (borderY * 2) +
                (cardHeight * 2) +
                (numberOfRows * cardHeight);

            // Card positions.
            for (var y = 0; y < numberOfRows; y++)
            {
                for (var x = 0; x < 10; x++)
                {
                    coordinates.Add((
                        borderX + (x * cardWidth),
                        (borderY + (cardHeight * 2)) +
                        (y * cardHeight)));
                }
            }

            // Use consistent path handling and avoid loading duplicates.
            SKBitmap LoadImage(string relativePath, bool decodeUrl = false)
            {
                if (decodeUrl)
                    relativePath = HttpUtility.UrlDecode(relativePath);

                var path = Path.Combine(
                    config.AppData.ImagePath, relativePath);

                if (imageCache.TryGetValue(path, out var cached))
                    return cached;

                if (!System.IO.File.Exists(path))
                    return null;

                var bitmap = SKBitmap.Decode(path);

                if (bitmap == null)
                    return null;

                if (imageCache.TryAdd(path, bitmap))
                    return bitmap;

                // Another thread may have inserted the same path.
                bitmap.Dispose();

                return imageCache.TryGetValue(path, out cached) ? cached : null;
            }

            try
            {
                // Preload images into the cache.
                if (QueryDeck.Hero != null)
                {
                    LoadImage(QueryDeck.Hero.LatestRevision.GetSmallImage);
                }

                foreach (var landscape in QueryDeck.Landscapes)
                {
                    LoadImage(landscape.LatestRevision.GetSmallImage);
                }

                foreach (var card in QueryDeck.Cards.OrderBy(x => x.LatestRevision.Name))
                {
                    LoadImage(
                        card.LatestRevision.GetSmallImage,
                        decodeUrl: true);
                }

                using var outputBitmap = new SKBitmap(
                    new SKImageInfo(
                        finalImageWidth,
                        finalImageHeight,
                        SKColorType.Rgba8888,
                        SKAlphaType.Premul));

                using var canvas = new SKCanvas(outputBitmap);

                // Preserve the transparent background of the original.
                canvas.Clear(SKColors.Transparent);

                var sampling = new SKSamplingOptions(
                    SKFilterMode.Linear,
                    SKMipmapMode.Linear);

                // Draw the hero.
                var coordCounter = 0;

                if (QueryDeck.Hero != null)
                {
                    var heroPath = Path.Combine(
                        config.AppData.ImagePath,
                        QueryDeck.Hero.LatestRevision.GetSmallImage);

                    if (imageCache.TryGetValue(heroPath, out var heroImage))
                    {
                        var loc = coordinates[coordCounter];

                        canvas.DrawBitmap(
                            heroImage,
                            new SKRect(
                                loc.X,
                                loc.Y,
                                loc.X + cardWidth * 2,
                                loc.Y + cardHeight * 2),
                            sampling);
                    }
                }

                coordCounter++;

                // Draw landscapes.
                foreach (var landscape in QueryDeck.Landscapes)
                {
                    var loc = coordinates[coordCounter];

                    var landPath = Path.Combine(
                        config.AppData.ImagePath,
                        landscape.LatestRevision.GetSmallImage);

                    if (imageCache.TryGetValue(
                        landPath, out var landImage))
                    {
                        canvas.DrawBitmap(
                            landImage,
                            new SKRect(
                                loc.X,
                                loc.Y,
                                loc.X + cardWidth * 2,
                                loc.Y + cardHeight * 2),
                            sampling);
                    }

                    coordCounter++;
                }

                // Draw cards in alphabetical order.
                foreach (var card in QueryDeck.Cards.OrderBy(x => x.LatestRevision.Name))
                {
                    var loc = coordinates[coordCounter];

                    var cardPath = Path.Combine(
                        config.AppData.ImagePath,
                        HttpUtility.UrlDecode(
                            card.LatestRevision.GetSmallImage));

                    if (imageCache.TryGetValue(
                        cardPath, out var cardImage))
                    {
                        canvas.DrawBitmap(
                            cardImage,
                            new SKRect(
                                loc.X,
                                loc.Y,
                                loc.X + cardWidth,
                                loc.Y + cardHeight),
                            sampling);
                    }

                    coordCounter++;
                }

                canvas.Flush();

                using var image = SKImage.FromBitmap(outputBitmap);
                using var encoded = image.Encode(
                    SKEncodedImageFormat.Png, 100);

                return encoded.ToArray();
            }
            finally
            {
                // Release all cached native image memory.
                foreach (var img in imageCache.Values)
                {
                    img.Dispose();
                }

                imageCache.Clear();
            }
        }

        public async Task<IActionResult> OnGet()
        {
            Cards = cardRepository.GetAll();

            await BuildDeckFromQuery();

            if (QueryDeck != null)
            {
                var image = CreateCoordinates();

                return File(image, "image/png", "download.png");
            }

            IsInvalid = true;
            return Page();
        }

        private async Task BuildDeckFromQuery()
        {
            var isLoggedIn = User.Identity.IsAuthenticated;
            User user = isLoggedIn ? await GetUser() : null;
            if (DeckId.HasValue)
            {
                bool accessible;
                if (User.Identity.IsAuthenticated)
                {
                    accessible = deckRepository.DoesUserHaveAccess(DeckId.Value, user.Id);
                }
                else
                {
                    accessible = deckRepository.IsPublic(DeckId.Value);
                }

                if (accessible)
                {
                    Deck deck = deckRepository.GetDeck(DeckId.Value);
                    if (deck != null)
                    {
                        QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards);
                    }
                }
            }
            else if (!string.IsNullOrWhiteSpace(GzDeck))
            {
                QueryDeck = QueryDeck.GetDeckFromGzip(GzDeck, Cards);
            }
            else if (!string.IsNullOrWhiteSpace(NonGZDeck))
            {
                QueryDeck = QueryDeck.GetFromQuery(NonGZDeck, Cards);
            }
        }
    }
}
