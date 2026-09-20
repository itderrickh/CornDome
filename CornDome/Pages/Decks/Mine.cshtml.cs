using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CornDome.Pages.Decks
{
    [Authorize]
    public class MineModel(MainContext mainContext, ICardRepository cardRepository, Config config, IUserRepository userRepository) : PageModel
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

            var totalDecks = await mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == loggedInUser.Id)
                .CountAsync();

            TotalPages = (int)Math.Ceiling(totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = await mainContext.Decks
                .Include(card => card.User)
                .Where(u => u.UserId == loggedInUser.Id)
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
        }
    }
}
