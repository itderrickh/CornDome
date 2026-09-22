using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Decks
{
    public class IndexModel(IDeckRepository deckRepository, ICardRepository cardRepository) : BasePageModel
    {
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Author { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CardId { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();

        public async Task OnGet()
        {
            var userId = -1;
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUser = await GetUser();
                userId = loggedInUser.Id;
            }
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            (int totalDecks, List<Deck> decks) = deckRepository.GetAllVisibleDecks(Author, CardId, PageNumber, PageSize);

            TotalPages = (int)Math.Ceiling(
                totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = decks;

            DeckGrid = new DeckGridViewModel
            {
                Decks = Decks,
                PageNumber = PageNumber,
                TotalPages = TotalPages,
                BaseUrl = BaseUrl,
                Cards = [.. cardRepository.GetAll()],
                UserId = userId
            };
        }
    }
}
