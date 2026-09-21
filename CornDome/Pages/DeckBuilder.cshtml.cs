using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class DeckBuilderModel(ICardRepository cardRepository, Config config, IUserRepository userRepository, MainContext mainContext) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;
        public bool QueryBuildFailed { get; set; } = false;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            try
            {
                if (Request.QueryString.HasValue)
                {
                    BuildDeckFromQuery();
                }
            }
            catch (Exception)
            {
                QueryBuildFailed = true;
            }
        }

        private void BuildDeckFromQuery()
        {
            if (DeckId.HasValue)
            {
                // TODO ADD PERMISSIONS
                var deck = mainContext.Decks.Where(x => x.Id == DeckId.Value).FirstOrDefault();
                if (deck != null)
                {
                    QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards);
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

        public async Task<IActionResult> OnPostSaveDeckToDatabaseAsync([FromBody] SaveDeckRequest request)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized();
            }

            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));

            if (identifier == null || loggedInUser == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid deck data.",
                    errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Value!.Errors.Select(e => e.ErrorMessage)
                        )
                });
            }

            var ungzDeckString = DeckEncoder.UrlToDeck(request.DeckString);

            mainContext.Decks.Add(new Deck()
            {
                Created = DateTime.Now,
                DeckString = ungzDeckString,
                Description = request.Description,
                IconCardId = request.IconId,
                Modified = DateTime.Now,
                UserId = loggedInUser.Id,
                Visibility = request.Visibility
            });
            mainContext.SaveChanges();

            return new JsonResult(new
            {
                success = true,
                message = "Deck saved successfully."
            });
        }

        public class SaveDeckRequest
        {
            [Required]
            public string DeckString { get; set; }
            [Required]
            public DeckVisibility Visibility { get; set; }
            [Required]
            [StringLength(400)]
            public string Description { get; set; }
            [Required]
            public int IconId { get; set; }
        }
    }
}
