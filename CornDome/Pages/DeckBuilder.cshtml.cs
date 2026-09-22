using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages
{
    public class DeckBuilderModel(ICardRepository cardRepository, IDeckRepository deckRepository) : BasePageModel
    {
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public bool QueryBuildFailed { get; set; } = false;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        public bool IsDbDeck { get; set; } = false;
        public bool IsMyDeck { get; set; } = false;
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public DeckVisibility Visibility { get; set; }
        [BindProperty]
        public int IconCard { get; set; }

        public async Task OnGet()
        {
            Cards = cardRepository.GetAll();

            try
            {
                await BuildDeckFromQuery();
            }
            catch (Exception)
            {
                QueryBuildFailed = true;
            }
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

                Deck deck = null;

                if (accessible)
                {
                    deck = deckRepository.GetDeck(DeckId.Value);

                    if (deck != null)
                    {
                        QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards);

                        if (isLoggedIn && deck.UserId == user.Id)
                        {
                            Description = ProfanityHelper.RelieveTheProfane(deck.Description);
                            IconCard = deck.IconCardId;
                            Visibility = deck.Visibility;
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
