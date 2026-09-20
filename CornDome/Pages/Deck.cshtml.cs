using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class DeckModel(ICardRepository cardRepository, Config config, IUserRepository userRepository, MainContext mainContext) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;

        public void OnGet()
        {
            Cards = _cardRepository.GetAll();

            if (Request.QueryString.HasValue)
            {
                BuildDeckFromQuery();
            }
        }

        private void BuildDeckFromQuery()
        {
            var nonGZDeck = Request.Query["deck"];
            var gzDeck = Request.Query["gzdeck"];

            if (string.IsNullOrEmpty(nonGZDeck))
            {
                QueryDeck = QueryDeck.GetDeckFromGzip(gzDeck, Cards);
            }
            else
            {
                QueryDeck = QueryDeck.GetFromQuery(nonGZDeck, Cards);
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

            mainContext.Decks.Add(new Deck()
            {
                Created = DateTime.Now,
                DeckString = request.DeckString,
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
