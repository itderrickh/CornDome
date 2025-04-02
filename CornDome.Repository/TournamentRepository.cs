using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface ITournamentRepository
    {
        int InsertTournament(Tournament tournament, List<TournamentResult> results);
        List<Tournament> GetAllTournaments();
    }

    public class TournamentRepository(IDbConnectionFactory dbConnectionFactory) : ITournamentRepository
    {
        private const string INSERT_TOURNAMENT_QUERY = @"
            INSERT INTO
	            Tournament (TournamentDate, Name, Description)
            VALUES 
	            (@TournamentDate, @TournamentName, @TournamentDescription);

            SELECT LAST_INSERT_ROWID();
        ";

        private const string INSERT_RESULT_QUERY = @"
            INSERT INTO
	            TournamentResult (TournamentId, Placement, Name, Decklist)
            VALUES 
	            (@TournamentId, @Placement, @Name, @Decklist);
        ";

        public int InsertTournament(Tournament tournament, List<TournamentResult> results)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentId = con.QueryFirstOrDefault<int>(INSERT_TOURNAMENT_QUERY, new { TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription });

            foreach (var resultItem in results)
            {
                if (!string.IsNullOrEmpty(resultItem.Decklist) || !string.IsNullOrEmpty(resultItem.Name))
                    con.Execute(INSERT_RESULT_QUERY, new { TournamentId = tournamentId, resultItem.Placement, resultItem.Name, resultItem.Decklist });
            }

            return tournamentId;
        }

        public List<Tournament> GetAllTournaments()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentResults = con.Query<TournamentResult>("SELECT Id, TournamentId, Placement, Name, Decklist FROM TournamentResult");
            var tournaments = con.Query<Tournament>("SELECT Id, TournamentDate, Name AS TournamentName, Description AS TournamentDescription from Tournament");

            foreach (var tournament in tournaments)
            {
                tournament.TournamentResults = tournamentResults.Where(x => x.TournamentId == tournament.Id).ToList();
            }

            return tournaments.ToList();
        }
    }
}
