using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

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

        [BindProperty(SupportsGet = true)]
        public string? Author { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CardId { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();

        public async Task OnGet()
        {
            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var query = mainContext.Decks
                .Include(deck => deck.User)
                .Where(deck => deck.Visibility == DeckVisibility.Visible)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(Author))
            {
                query = query.Where(deck =>
                    deck.User.UserName != null &&
                    deck.User.UserName.Contains(Author));
            }

            if (CardId.HasValue)
            {
                query = query.Where(deck => deck.DeckString.Contains(CardId.Value.ToString() + ":") || deck.DeckString.StartsWith(CardId.Value.ToString() + ";"));
            }

            query = query.OrderByDescending(deck => deck.Modified);

            var totalDecks = await query.CountAsync();

            TotalPages = (int)Math.Ceiling(
                totalDecks / (double)PageSize);

            if (TotalPages > 0 && PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Decks = await query
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
