using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Pages.Decks
{
    public class IndexModel(MainContext mainContext, ICardRepository cardRepository, Config config) : PageModel
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
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var totalDecks = await mainContext.Decks
                .Where(v => v.Visibility == DeckVisibility.Visible)
                .CountAsync();

            TotalPages = (int)Math.Ceiling(totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = await mainContext.Decks
                .Include(deck => deck.User)
                .Where(deck => deck.Visibility == DeckVisibility.Visible)
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
