using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
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
        public void CreateCoordinates()
        {
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

            if (QueryDeck.Hero != null)
            {
                var heroLoc = coordinates[coordCounter];
                using var heroImage = Image.Load<Rgba32>(Path.Combine(config.AppData.ImagePath, QueryDeck.Hero.ImageUrl));
                heroImage.Mutate(ci => ci.Resize(new Size(cardWidth * 2, cardHeight * 2)));

                outputImage.Mutate(o => o.DrawImage(heroImage, new Point(heroLoc.Item1, heroLoc.Item2), 1f));
            }

            // Hero or blank added
            coordCounter++;

            foreach (var landscape in QueryDeck.Landscapes)
            {
                var landLoc = coordinates[coordCounter];
                using var landImage = Image.Load<Rgba32>(Path.Combine(config.AppData.ImagePath, landscape.ImageUrl));
                landImage.Mutate(ci => ci.Resize(new Size(cardWidth * 2, cardHeight * 2)));

                outputImage.Mutate(o => o.DrawImage(landImage, new Point(landLoc.Item1, landLoc.Item2), 1f).BackgroundColor(Color.DarkGray));
                coordCounter++;
            }
            
            // Leave landscape area blank if less than 4
            if (QueryDeck.Landscapes.Count < 4)
                coordCounter += (4 - QueryDeck.Landscapes.Count);
            
            foreach (var card in QueryDeck.Cards)
            {
                var cardLoc = coordinates[coordCounter];
                using var cardImage = Image.Load<Rgba32>(Path.Combine(config.AppData.ImagePath, HttpUtility.UrlDecode(card.ImageUrl)));
                cardImage.Mutate(ci => ci.Resize(new Size(cardWidth, cardHeight)));

                outputImage.Mutate(o => o.DrawImage(cardImage, new Point(cardLoc.Item1, cardLoc.Item2), 1f));
                coordCounter++;
            }

            ImageString = outputImage.ToBase64String(PngFormat.Instance);
        }
        
        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
                QueryDeck = Deck.GetFromQuery(Request.Query["deck"], Cards);

            CreateCoordinates();
        }
    }
}
