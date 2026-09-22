using CornDome.Models;
using CornDome.Models.Tournaments;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Tournaments
{
    [Authorize]
    public class RegisterModel(TournamentContext tournamentContext) : BasePageModel
    {
        [BindProperty]
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public TournamentRegistration ActiveRegistration { get; set; }

        [BindProperty]
        public string Deck { get; set; }

        public async Task OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);
            Tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);

            var user = await GetUser();
            var userId = user.Id;
            var registration = tournamentContext.Registrations.FirstOrDefault(x => x.UserId == userId && x.TournamentId == TournamentId);
            ActiveRegistration = registration;
        }

        public async Task<IActionResult> OnPostCreateRegistration()
        {
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "There was an issue while trying to register";
                return Page();
            }

            Deck = Deck.Replace("http://carddweeb.com/Deck?deck=", "")
                .Replace("https://carddweeb.com/Deck?deck=", "")
                .Replace("http://www.carddweeb.com/Deck?deck=", "")
                .Replace("https://www.carddweeb.com/Deck?deck=", "");

            var user = await GetUser();
            var userId = user.Id;
            if (user != null)
            {
                var registration = new TournamentRegistration()
                {
                    Deck = Deck,
                    TournamentId = TournamentId,
                    UserId = userId
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
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "There was an issue while trying to register";
                return Page();
            }

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
                // Handle the case when they resubmit the same list
                if (registration.Deck == Deck)
                {
                    TempData["Status"] = "success";
                    TempData["Message"] = "Updated registration successfully!";
                    return Page();
                }

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

        public async Task<IActionResult> OnPostCancelRegistration()
        {
            var tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);

            var user = await GetUser();
            var userId = user.Id;

            var updated = false;
            var isTournamentEditable = tournament.Status == TournamentStatus.OpenForSignups || tournament.Status == TournamentStatus.ClosedForSignups;
            var isValidToUpdate = isTournamentEditable;
            if (isValidToUpdate)
            {
                var registration = tournamentContext.Registrations.FirstOrDefault(x => x.UserId == userId && x.TournamentId == TournamentId);
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
