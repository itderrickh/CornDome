using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Collections.Concurrent;
using System.Web;

namespace CornDome.Pages
{
    public class DownloadModel(Config configuration, ICardRepository cardRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        private readonly Config config = configuration;
        public IEnumerable<Card> Cards { get; set; }
        public Deck QueryDeck { get; set; } = null;

        private const int cardWidth = 86;
        private const int cardHeight = 120;
        private const int finalImageWidth = (borderX * 2) + (cardWidth * 10);
        private int finalImageHeight;
        private const int borderX = 20;
        private const int borderY = 20;

        public string ImageString { get; set; }
        public ConcurrentDictionary<string, Image<Rgb24>> imageCache = new();
        public byte[] CreateCoordinates()
        {
            Parallel.ForEach(QueryDeck.Cards, card =>
            {
                var cardPath = Path.Combine(config.AppData.ImagePath, HttpUtility.UrlDecode(card.LatestRevision.GetSmallImage));
                if (!imageCache.ContainsKey(cardPath) && System.IO.File.Exists(cardPath))
                {
                    imageCache[cardPath] = Image.Load<Rgb24>(cardPath);
                }
            });

            // X, Y
            var coordinates = new List<(int, int)>();
            for (var topX = 0; topX < 5; topX++)
            {
                coordinates.Add((borderX + (topX * cardWidth * 2), borderY));
            }

            var numberOfRows = (int)Math.Round(QueryDeck.Cards.Count / 10m, MidpointRounding.ToPositiveInfinity);
            finalImageHeight = ((borderY * 2) + (cardHeight * 2) + (numberOfRows * cardHeight));

            for (var y = 0; y < numberOfRows; y++)
            {
                for (var x = 0; x < 10; x++)
                {
                    coordinates.Add((borderX + (x * cardWidth), (borderY + (cardHeight * 2)) + (y * cardHeight)));
                }
            }

            var coordCounter = 0;
            using var outputImage = new Image<Rgba32>(finalImageWidth, finalImageHeight);

            // Load Hero Image
            if (QueryDeck.Hero != null)
            {
                var heroPath = Path.Combine(config.AppData.ImagePath, QueryDeck.Hero.LatestRevision.GetSmallImage);
                if (System.IO.File.Exists(heroPath))
                {
                    imageCache[heroPath] = Image.Load<Rgb24>(heroPath);
                }
            }

            // Load Landscape Images
            foreach (var landscape in QueryDeck.Landscapes)
            {
                var landPath = Path.Combine(config.AppData.ImagePath, landscape.LatestRevision.GetSmallImage);
                if (!imageCache.ContainsKey(landPath) && System.IO.File.Exists(landPath))
                {
                    imageCache[landPath] = Image.Load<Rgb24>(landPath);
                }
            }

            // Load Card Images
            foreach (var card in QueryDeck.Cards)
            {
                var cardPath = Path.Combine(config.AppData.ImagePath, HttpUtility.UrlDecode(card.LatestRevision.GetSmallImage));
                if (!imageCache.ContainsKey(cardPath) && System.IO.File.Exists(cardPath))
                {
                    imageCache[cardPath] = Image.Load<Rgb24>(cardPath);
                }
            }

            // **DRAW IMAGES FROM CACHE**
            if (QueryDeck.Hero != null && imageCache.TryGetValue(Path.Combine(config.AppData.ImagePath, QueryDeck.Hero.LatestRevision.GetSmallImage), out var heroImage))
            {
                var heroLoc = coordinates[coordCounter];
                heroImage.Mutate(ci => ci.Resize(new Size(cardWidth * 2, cardHeight * 2)));
                outputImage.Mutate(o => o.DrawImage(heroImage, new Point(heroLoc.Item1, heroLoc.Item2), 1f));
            }
            coordCounter++;

            foreach (var landscape in QueryDeck.Landscapes)
            {
                var landLoc = coordinates[coordCounter];
                var landPath = Path.Combine(config.AppData.ImagePath, landscape.LatestRevision.GetSmallImage);
                if (imageCache.TryGetValue(landPath, out var landImage))
                {
                    landImage.Mutate(ci => ci.Resize(new Size(cardWidth * 2, cardHeight * 2)));
                    outputImage.Mutate(o => o.DrawImage(landImage, new Point(landLoc.Item1, landLoc.Item2), 1f));
                }
                coordCounter++;
            }

            foreach (var card in QueryDeck.Cards)
            {
                var cardLoc = coordinates[coordCounter];
                var cardPath = Path.Combine(config.AppData.ImagePath, HttpUtility.UrlDecode(card.LatestRevision.GetSmallImage));
                if (imageCache.TryGetValue(cardPath, out var cardImage))
                {
                    cardImage.Mutate(ci => ci.Resize(new Size(cardWidth, cardHeight)));
                    outputImage.Mutate(o => o.DrawImage(cardImage, new Point(cardLoc.Item1, cardLoc.Item2), 1f));
                }
                coordCounter++;
            }

            // Dispose of cached images to free memory
            foreach (var img in imageCache.Values)
            {
                img.Dispose();
            }

            using var stream = new MemoryStream();
            outputImage.Save(stream, new PngEncoder());
            stream.Seek(0, SeekOrigin.Begin);

            return stream.ToArray();
        }
        
        public IActionResult OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
                QueryDeck = Deck.GetFromQuery(Request.Query["deck"], Cards);

            var image = CreateCoordinates();

            return File(image, "image/png", "download.png");
        }
    }
}
