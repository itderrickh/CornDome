using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Main;
using CornDome.Models.Users;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using CornDome.Repository.Users;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Printer
{
    public class PDFModel(ICardRepository cardRepository, IDeckRepository deckRepository, Config config) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }
        public bool IsDbDeck { get; set; } = false;
        public bool IsMyDeck { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            Cards = cardRepository.GetAll();

            await BuildDeckFromQuery();

            var pdfCreator = new PDFCreator(config);
            var printableCards = new List<PrintableCard>();
            if (QueryDeck.Hero != null)
            {
                printableCards.Add(
                new()
                {
                    ImagePath = QueryDeck.Hero.GetOptimizedImage,
                    CardType = (CardTypeEnum)QueryDeck.Hero.TypeId
                });
            }
            
            if (QueryDeck.Landscapes != null && QueryDeck.Landscapes.Count > 0)
            {
                printableCards.AddRange(QueryDeck.Landscapes.Select(x => new PrintableCard() { ImagePath = x.GetOptimizedImage, CardType = (CardTypeEnum)x.TypeId }));
            }

            if (QueryDeck.Cards != null && QueryDeck.Cards.Count > 0)
            {
                printableCards.AddRange(QueryDeck.Cards.Select(x => new PrintableCard() { ImagePath = x.GetOptimizedImage, CardType = (CardTypeEnum)x.TypeId }));
            }

            byte[] pdf = await pdfCreator.GeneratePdf(printableCards);

            return File(
                pdf,
                "application/pdf",
                "PrintableCards.pdf");
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

                        if (isLoggedIn && deck.UserId == user.Id)
                        {
                            IsMyDeck = true;
                        }

                        IsDbDeck = true;
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
