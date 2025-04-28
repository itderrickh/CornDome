using CornDome.Models.Tournaments;
using CornDome.Repository;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    [Authorize(Policy = "admin")]
    public class EditModel(TournamentContext tournamentContext) : PageModel
    {
        [BindProperty]
        public Tournament Tournament { get; set; }

        public int TournamentId { get; set; }
        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            Tournament = tournamentContext.Tournaments.SingleOrDefault(x => x.Id == TournamentId);
        }

        public IActionResult OnPostEditTournament()
        {
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Updated";
                return Page();
            }

            var result = false;
            var tournamentToUpdate = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == Tournament.Id);

            if (tournamentToUpdate != null)
            {
                tournamentToUpdate.TournamentDescription = Tournament.TournamentDescription;
                tournamentToUpdate.TournamentDate = Tournament.TournamentDate;
                tournamentToUpdate.TournamentName = Tournament.TournamentName;
                tournamentToUpdate.Status = Tournament.Status;
                result = tournamentContext.SaveChanges() > 0;
            }

            if (!result)
            { 
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Updated";
                return Page();
            }

            return RedirectToPage("/Tournaments/Index");
        }
    }
}
