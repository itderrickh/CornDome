using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Pages.Decks
{
    public class UserModel(MainContext mainContext, ICardRepository cardRepository, Config config, IUserRepository userRepository) : PageModel
    {
        public List<Card> Cards { get; set; }
        public List<Deck> Decks { get; set; }
        public string BaseUrl { get; set; } = config.BaseUrl;
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? UserId { get; set; }
        public string DisplayUsername { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (UserId == null)
            {
                return Page();
            }

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var user = await userRepository.GetUserById(UserId.Value);
            DisplayUsername = user.UserName;

            var totalDecks = await mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == UserId && u.Visibility == DeckVisibility.Visible)
                .CountAsync();

            TotalPages = (int)Math.Ceiling(totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = await mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == UserId && u.Visibility == DeckVisibility.Visible)
                .OrderByDescending(deck => deck.Modified)
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            DeckGrid = new DeckGridViewModel
            {
                Decks = Decks,
                PageNumber = PageNumber,
                TotalPages = TotalPages,
                BaseUrl = BaseUrl,
                Cards = [.. cardRepository.GetAll()]
            };

            return Page();
        }
    }
}
