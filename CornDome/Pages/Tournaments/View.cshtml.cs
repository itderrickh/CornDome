using CornDome.Models.Tournaments;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CornDome.Pages.Tournaments
{
    public class ViewModel(ITournamentRepository tournamentRepository) : PageModel
    {
        public Tournament Tournament { get; set; }
        public List<TournamentRegistration> RegisteredUsers { get; set; }

        public int TournamentId { get; set; }
        public void OnGet()
        {
            var queryId = Request.Query["id"];
            TournamentId = int.Parse(queryId);

            Tournament = tournamentRepository.GetById(TournamentId);
            RegisteredUsers = tournamentRepository.GetAllRegisteredUsers(TournamentId);
        }
    }
}
