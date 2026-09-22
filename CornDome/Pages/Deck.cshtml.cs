using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace CornDome.Pages
{
    public class DeckModel(ICardRepository cardRepository, Config config, IUserRepository userRepository, IDeckRepository deckRepository) : PageModel
    {
        private readonly ICardRepository _cardRepository = cardRepository;
        public IEnumerable<Card> Cards { get; set; }
        public QueryDeck QueryDeck { get; set; } = null;
        public string BaseUrl { get; set; } = config.BaseUrl;
        [BindProperty(Name = "id", SupportsGet = true)]
        public int? DeckId { get; set; }

        [BindProperty(Name = "gzdeck", SupportsGet = true)]
        public string GzDeck { get; set; }

        [BindProperty(Name = "deck", SupportsGet = true)]
        public string NonGZDeck { get; set; }

        public bool IsDbDeck { get; set; } = false;
        public bool IsMyDeck { get; set; } = false;
        [BindProperty]
        public string Description { get; set; }
        [BindProperty]
        public DeckVisibility Visibility { get; set; }
        [BindProperty]
        public int IconCard { get; set; }

        public async Task OnGet()
        {
            Cards = _cardRepository.GetAll();

            await BuildDeckFromQuery();
        }

        private async Task<User> GetUser()
        {
            var identifier = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = await userRepository.GetUserById(int.Parse(identifier));
            return loggedInUser;
        }

        private async Task BuildDeckFromQuery()
        {
            var isLoggedIn = User.Identity.IsAuthenticated;
            User user = isLoggedIn ? await GetUser() : null;
            if (DeckId.HasValue)
            {
                var accessible = false;

                if (User.Identity.IsAuthenticated)
                {
                    accessible = deckRepository.DoesUserHaveAccess(DeckId.Value, user.Id);
                }
                else
                {
                    accessible = deckRepository.IsPublic(DeckId.Value);
                }                

                Deck deck = null;

                if (accessible)
                {
                    deck = deckRepository.GetDeck(DeckId.Value);

                    if (deck != null)
                    {
                        QueryDeck = QueryDeck.GetFromString(deck.DeckString, Cards);

                        if (isLoggedIn && deck.UserId == user.Id)
                        {
                            Description = ProfanityHelper.RelieveTheProfane(deck.Description);
                            IconCard = deck.IconCardId;
                            Visibility = deck.Visibility;
                            IsMyDeck = true;
                        }
                        IsDbDeck = true;
                    }
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

            var isSuccess = false;
            // This deck exists already
            if (request.DeckId.HasValue)
            {
                var deck = deckRepository.GetDeck(request.DeckId.Value);
                // My deck
                if (deck != null && deck.UserId == loggedInUser.Id)
                {
                   isSuccess = deckRepository.ChangeDeckSettings(deck.Id, request.Visibility, ProfanityHelper.RelieveTheProfane(request.Description), request.IconId);
                }
                // Someone elses deck
                else if (deck != null && deck.UserId != loggedInUser.Id)
                {
                    isSuccess = deckRepository.AddDeck(new Deck()
                    {
                        Created = DateTime.Now,
                        DeckString = request.DeckString,
                        Description = ProfanityHelper.RelieveTheProfane(request.Description),
                        IconCardId = request.IconId,
                        Modified = DateTime.Now,
                        UserId = loggedInUser.Id,
                        Visibility = request.Visibility
                    });
                }
            } 
            else
            {
                isSuccess = deckRepository.AddDeck(new Deck()
                {
                    Created = DateTime.Now,
                    DeckString = request.DeckString,
                    Description = ProfanityHelper.RelieveTheProfane(request.Description),
                    IconCardId = request.IconId,
                    Modified = DateTime.Now,
                    UserId = loggedInUser.Id,
                    Visibility = request.Visibility
                });
            }

            if (isSuccess)
            {
                return new JsonResult(new
                {
                    success = true,
                    message = "Deck saved successfully."
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Deck did not save successfully."
                });
            }
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
            public int? DeckId { get; set; }
        }
    }
}
