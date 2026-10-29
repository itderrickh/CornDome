using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Models.Users;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CornDome.Controllers
{
    public class SaveDeckRequest
    {
        [Required(ErrorMessage = "Deck cannot be empty")]
        public string DeckContent { get; set; }
        [Required]
        public DeckVisibility Visibility { get; set; }
        [StringLength(400)]
        public string Description { get; set; }
        [Required]
        public int IconId { get; set; }
        public int? DeckId { get; set; }
        [Required]
        [StringLength(100)]
        public string DeckName { get; set; }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DecksController(UserManager<User> userManager, IDeckRepository deckRepository) : AuthorizedBaseController(userManager)
    {
        [HttpGet]
        [AllowAnonymous]
        [RequireUserAgent]
        public async Task<IActionResult> GetDecks([FromQuery] int limit = ApiPagination.DefaultLimit, [FromQuery] int offset = ApiPagination.DefaultOffset)
        {
            if (limit < 1 || limit > 100)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid limit",
                    Detail = $"Limit must be between 1 and {100}.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            if (offset < 0)
            {
                return BadRequest(new ProblemDetails
                {
                    Title = "Invalid offset",
                    Detail = "Offset must be zero or greater.",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var stopwatch = Stopwatch.StartNew();
            (int count, List<Deck> decks) = deckRepository.GetAllVisibleDecks(null, null, offset, limit);
            var serializedDecks = decks.Select(x => new SerializableDeck(x));
            stopwatch.Stop();

            var response = new ApiCollectionResponse<object>
            {
                Data = serializedDecks.Cast<object>().ToList(),
                Elapsed = stopwatch.Elapsed.TotalMilliseconds,
                Next = ApiPagination.GetNextUrl(Request, count, limit, offset),
                Total = count,
                Count = serializedDecks.Count(),
                Limit = limit,
                Offset = offset
            };

            return Ok(response);
        }
        
        [HttpDelete("{deckId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveDeck(int deckId)
        {
            var loggedInUser = await GetUser();
            if (loggedInUser == null)
            {
                return Unauthorized();
            }

            var deck = deckRepository.GetDeck(deckId);
            if (deck != null && deck.UserId == loggedInUser.Id)
            {
                var success = deckRepository.DeleteDeck(deckId);
                if (success)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateDeck([FromBody] SaveDeckRequest request)
        {
            var loggedInUser = await GetUser();
            if (loggedInUser == null)
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
                    isSuccess = deckRepository.ChangeDeckSettings(deck.Id, request.Visibility, request.Description, request.IconId, ProfanityHelper.RelieveTheProfane(request.DeckName));
                    isSuccess = deckRepository.ChangeDeckValue(deck.Id, request.DeckContent);
                }
                // Someone elses deck
                else if (deck != null && deck.UserId != loggedInUser.Id)
                {
                    isSuccess = deckRepository.AddDeck(new Deck()
                    {
                        Created = DateTime.Now,
                        DeckString = request.DeckContent,
                        DeckName = ProfanityHelper.RelieveTheProfane(request.DeckName),
                        Description = request.Description,
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
                    DeckString = request.DeckContent,
                    DeckName = ProfanityHelper.RelieveTheProfane(request.DeckName),
                    Description = request.Description,
                    IconCardId = request.IconId,
                    Modified = DateTime.Now,
                    UserId = loggedInUser.Id,
                    Visibility = request.Visibility
                });
            }

            if (isSuccess)
            {
                return Ok(new
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
    }
}
