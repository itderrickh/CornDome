using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;

namespace CornDome.Pages.Admin
{
    public class BuildDecklistModel(ICardRepository cardRepository) : PageModel
    {
        [BindProperty]
        public string Query { get; set; }

        public string ErrorMessage { get; set; }

        public IEnumerable<CardFullDetails> Cards { get; set; }
        public Dictionary<string, int> Deck { get; set; } = [];
        public string DeckSlug { get; set; }

        public IActionResult OnPost()
        {
            var dbQueryLines = new List<string>();
            var lines = Query.Split("\r\n");
            foreach (var line in lines)
            {
                if (line.StartsWith("//") || string.IsNullOrEmpty(line))
                    continue;

                var filteredString = line.Substring(2)
                    .Replace("Landscape", "")
                    .ToLower()
                    .Trim();
                dbQueryLines.Add(filteredString);

                var count = int.Parse(line[..1]);
                Deck.Add(filteredString, count);
            }

            var queryCards = cardRepository.GetCardsFromQuery(dbQueryLines);
            
            // Filter out multiple landscapes
            var allLandscapes = queryCards.Where(x => x.LatestRevision.CardType == CardType.Landscape);
            var exactLandscapes = allLandscapes.GroupBy(x => x.LatestRevision.Name).Select(x => x.OrderBy(y => y.Id).First());

            var realCards = queryCards.Where(x => x.LatestRevision.CardType != CardType.Landscape);

            Cards = realCards.Union(exactLandscapes);

            var hero = Cards.FirstOrDefault(x => x.LatestRevision.CardType == CardType.Hero);
            var landscapes = Cards.Where(x => x.LatestRevision.CardType == CardType.Landscape);
            var cards = Cards.Where(x => x.LatestRevision.CardType != CardType.Hero && x.LatestRevision.CardType != CardType.Landscape);

            var utf8String = Encoding.UTF8.GetBytes(string.Join(";", hero.LatestRevision.CardId, PackString(landscapes), PackString(cards)));
            DeckSlug = Convert.ToBase64String(utf8String);

            return Page();
        }

        private string PackString(IEnumerable<CardFullDetails> cards)
        {
            var cardStrings = cards.Select(x => x.LatestRevision.CardId + ":" + Deck[x.LatestRevision.Name.ToLower()]);
            return string.Join(',', cardStrings);
        }
    }
}
