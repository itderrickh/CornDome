using CornDome.Models.Tournaments;
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
        [BindProperty]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public TournamentRegistration ActiveRegistration { get; set; }

        [BindProperty]
        public string Deck { get; set; }

        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);
            Tournament = tournamentRepository.GetById(TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userIdValid = int.TryParse(userId, out int integerUserId);
            var registration = tournamentRepository.GetRegistration(integerUserId, TournamentId);
            ActiveRegistration = registration;
        }

        public IActionResult OnPostCreateRegistration()
        {
            Deck = Deck.Replace("http://carddweeb.com/Deck?deck=", "")
                .Replace("https://carddweeb.com/Deck?deck=", "")
                .Replace("http://www.carddweeb.com/Deck?deck=", "")
                .Replace("https://www.carddweeb.com/Deck?deck=", "");

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

        public IActionResult OnPostUpdateRegistration()
        {
            Deck = Deck.Replace("http://carddweeb.com/Deck?deck=", "")
                .Replace("https://carddweeb.com/Deck?deck=", "")
                .Replace("http://www.carddweeb.com/Deck?deck=", "")
                .Replace("https://www.carddweeb.com/Deck?deck=", "");

            var tournament = tournamentRepository.GetById(TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var updated = false;

            var isUserValid = userId != null && int.TryParse(userId, out int integerUserId);
            var isTournamentEditable = tournament.Status == TournamentStatus.OpenForSignups || tournament.Status == TournamentStatus.ClosedForSignups;
            var isValidToUpdate = isTournamentEditable && isUserValid;
            if (isValidToUpdate)
            {
                var registration = tournamentRepository.GetRegistration(int.Parse(userId), TournamentId);

                registration.Deck = Deck;
                updated = tournamentRepository.UpdateRegistration(registration);

                if (updated)
                {
                    TempData["Status"] = "success";
                    TempData["Message"] = "Updated registration successfully!";
                    return Page();
                }
            }

            TempData["Status"] = "danger";
            TempData["Message"] = "There was an issue while trying to update registration";
            return Page();
        }

        public IActionResult OnPostCancelRegistration()
        {
            var tournament = tournamentRepository.GetById(TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var updated = false;
            var isUserValid = userId != null && int.TryParse(userId, out int integerUserId);
            var isTournamentEditable = tournament.Status == TournamentStatus.OpenForSignups || tournament.Status == TournamentStatus.ClosedForSignups;
            var isValidToUpdate = isTournamentEditable && isUserValid;
            if (isValidToUpdate)
            {
                updated = tournamentRepository.DeleteRegistration(int.Parse(userId), TournamentId);

                if (updated)
                {
                    TempData["Status"] = "success";
                    TempData["Message"] = "Successfully dropped!";
                    return Page();
                }
            }

            TempData["Status"] = "danger";
            TempData["Message"] = "There was an issue while trying to cancel registration";
            return Page();
        }
    }
}
