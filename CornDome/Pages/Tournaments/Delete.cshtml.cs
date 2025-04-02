using CornDome.Models.Tournaments;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    public class DeleteModel(ITournamentRepository tournamentRepository) : PageModel
    {
        [BindProperty]
        public Tournament Tournament { get; set; }

        public int TournamentId { get; set; }
        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            Tournament = tournamentRepository.GetById(TournamentId);
        }

        public IActionResult OnPostDeleteTournament()
        {
            if (!ModelState.IsValid)
            {
                TempData["Status"] = "danger";
                TempData["Message"] = "Tournament Not Deleted";
                return Page();
            }

            var result = tournamentRepository.DeleteTournament(Tournament.Id);

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
