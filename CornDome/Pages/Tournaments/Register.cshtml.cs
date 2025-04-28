using CornDome.Models.Tournaments;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Tournaments
{
    [Authorize]
    public class RegisterModel(TournamentContext tournamentContext) : PageModel
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
            Tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userIdValid = int.TryParse(userId, out int integerUserId);
            var registration = tournamentContext.Registrations.FirstOrDefault(x => x.UserId == integerUserId && x.TournamentId == TournamentId);
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

                tournamentContext.Registrations.Add(registration);
                var result = tournamentContext.SaveChanges();
                if (result > 0)
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

            var tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var updated = false;

            var isUserValid = userId != null && int.TryParse(userId, out int integerUserId);
            var isTournamentEditable = tournament.Status == TournamentStatus.OpenForSignups || tournament.Status == TournamentStatus.ClosedForSignups;
            var isValidToUpdate = isTournamentEditable && isUserValid;
            if (isValidToUpdate)
            {
                var registration = tournamentContext.Registrations.FirstOrDefault(x => x.UserId == int.Parse(userId) && x.TournamentId == TournamentId);

                registration.Deck = Deck;
                updated = tournamentContext.SaveChanges() > 0;

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
            var tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);

            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            var updated = false;
            var isUserValid = userId != null && int.TryParse(userId, out int integerUserId);
            var isTournamentEditable = tournament.Status == TournamentStatus.OpenForSignups || tournament.Status == TournamentStatus.ClosedForSignups;
            var isValidToUpdate = isTournamentEditable && isUserValid;
            if (isValidToUpdate)
            {
                var registration = tournamentContext.Registrations.FirstOrDefault(x => x.UserId == int.Parse(userId) && x.TournamentId == TournamentId);
                tournamentContext.Registrations.Remove(registration);

                updated = tournamentContext.SaveChanges() > 0;

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
