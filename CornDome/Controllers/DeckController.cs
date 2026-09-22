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

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DeckController(UserManager<User> userManager, IDeckRepository deckRepository) : AuthorizedBaseController(userManager)
    {
        [HttpPost]
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
                    isSuccess = deckRepository.ChangeDeckSettings(deck.Id, request.Visibility, ProfanityHelper.RelieveTheProfane(request.Description), request.IconId);
                    isSuccess = deckRepository.ChangeDeckValue(deck.Id, request.DeckString);
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
