using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Decks
{
    [Authorize]
    public class MineModel(ICardRepository cardRepository, IDeckRepository deckRepository) : BasePageModel
    {
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();

        public async Task OnGet()
        {
            var loggedInUser = await GetUser();

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var totalDecks = deckRepository.GetTotalUserDecks(loggedInUser.Id);

            TotalPages = (int)Math.Ceiling(totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = deckRepository.GetUserDecks(loggedInUser.Id, PageNumber, PageSize);

            DeckGrid = new DeckGridViewModel
            {
                Decks = Decks,
                PageNumber = PageNumber,
                TotalPages = TotalPages,
                BaseUrl = BaseUrl,
                Cards = [.. cardRepository.GetAll()],
                UserId = loggedInUser.Id
            };
        }
    }
}
