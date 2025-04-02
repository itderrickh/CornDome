using CornDome.Models;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Tournaments
{
    [Authorize]
    public class RegisterModel(ITournamentRepository tournamentRepository) : PageModel
    {
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }

        [BindProperty]
        public string Deck { get; set; }

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);
            Tournament = tournamentRepository.GetById(TournamentId);
        }

        public IActionResult OnPost()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId != null && int.TryParse(userId, out int integerUserId))
            {
                var registration = new TournamentRegistration()
                {
                    Deck = Deck,
                    TournamentId = TournamentId,
                    UserId = integerUserId
                };

                var result = tournamentRepository.RegisterForTournament(Tournament, registration);
                if (result)
                {
                    TempData["Status"] = "success";
                    TempData["Message"] = "Successfully registered!";
                    return Page();
                }
            }

            TempData["Status"] = "danger";
            TempData["Message"] = "There was an issue while trying to register";
            return Page();
        }
    }
}
