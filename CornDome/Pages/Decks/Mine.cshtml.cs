using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Decks
{
    [Authorize]
    public class MineModel(ICardRepository cardRepository, Config config, IUserRepository userRepository, IDeckRepository deckRepository) : PageModel
    {
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }
        public string BaseUrl { get; set; } = config.BaseUrl;
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();

        public async Task OnGet()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

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
