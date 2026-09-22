using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;

namespace CornDome.Pages.Decks
{
    public class IndexModel(IDeckRepository deckRepository, ICardRepository cardRepository, IUserRepository userRepository, Config config) : PageModel
    {
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }
        public string BaseUrl { get; set; } = config.BaseUrl;

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Author { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CardId { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();

        public async Task OnGet()
        {
            var userId = -1;
            if (User.Identity.IsAuthenticated)
            {
                var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));
                userId = loggedInUser.Id;
            }
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            (int totalDecks, List<Deck> decks) results = deckRepository.GetAllVisibleDecks(Author, CardId, PageNumber, PageSize);

            TotalPages = (int)Math.Ceiling(
                results.totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = results.decks;

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
