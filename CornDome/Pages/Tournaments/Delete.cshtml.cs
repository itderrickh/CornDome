using CornDome.Models.Tournaments;
using CornDome.Repository.Tournaments;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    public class DeleteModel(TournamentContext tournamentContext) : PageModel
    {
        [BindProperty]
        public Tournament Tournament { get; set; }

        public int TournamentId { get; set; }
        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            Tournament = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == TournamentId);
        }

        public IActionResult OnPostDeleteTournament()
        {
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Deleted";
                return Page();
            }


            var result = false;
            var tournamentToDelete = tournamentContext.Tournaments.FirstOrDefault(x => x.Id == Tournament.Id);

            if (tournamentToDelete != null)
            {
                tournamentContext.Remove(tournamentToDelete);
                result = tournamentContext.SaveChanges() > 0;
            }

            if (!result)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Deleted";
                return Page();
            }

            return RedirectToPage("/Tournaments/Index");
        }
    }
}
