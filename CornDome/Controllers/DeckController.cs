using CornDome.Helpers;
using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    public class DeckController(UserManager<User> userManager, IDeckRepository deckRepository) : AuthorizedBaseController(userManager)
    {
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
                    isSuccess = deckRepository.ChangeDeckSettings(deck.Id, request.Visibility, ProfanityHelper.RelieveTheProfane(request.Description), request.IconId, ProfanityHelper.RelieveTheProfane(request.DeckName));
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
                    DeckString = request.DeckContent,
                    DeckName = ProfanityHelper.RelieveTheProfane(request.DeckName),
                    Description = ProfanityHelper.RelieveTheProfane(request.Description),
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
