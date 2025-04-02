using CornDome.Models;
using Dapper;

namespace CornDome.Repository
{
    public interface ITournamentRepository
    {
        int InsertTournament(Tournament tournament);
        int InsertTournament(Tournament tournament, List<TournamentResult> results);
        bool UpdateTournament(int id, Tournament tournament);
        List<Tournament> GetAllTournaments();
        Tournament GetById(int id);
        bool RegisterForTournament(Tournament tournament, TournamentRegistration registration);
    }

    public class TournamentRepository(IDbConnectionFactory dbConnectionFactory) : ITournamentRepository
    {
        private const string INSERT_TOURNAMENT_QUERY = @"
            INSERT INTO
	            Tournament (TournamentDate, Name, Description, Status)
            VALUES 
	            (@TournamentDate, @TournamentName, @TournamentDescription, @Status);

            SELECT LAST_INSERT_ROWID();
        ";

        private const string INSERT_RESULT_QUERY = @"
            INSERT INTO
	            TournamentResult (TournamentId, Placement, Name, Decklist)
            VALUES 
	            (@TournamentId, @Placement, @Name, @Decklist);
        ";

        private const string UPDATE_TOURNAMENT_QUERY = @"
            UPDATE Tournament
            SET
                TournamentDate = @TournamentDate,
                Name = @TournamentName,
                Description = @TournamentDescription,
                Status = @Status
            WHERE Id = @TournamentId;
        ";

        private const string REGISTER_TOURNAMENT_QUERY = @"
            INSERT INTO
                TournamentRegistration (UserId, TournamentId, Deck)
            VALUES
                (@UserId, @TournamentId, @Deck);
        ";

        public int InsertTournament(Tournament tournament)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentId = con.QueryFirstOrDefault<int>(INSERT_TOURNAMENT_QUERY, new { TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription, Status = TournamentStatus.NotActive });

            return tournamentId;
        }

        public int InsertTournament(Tournament tournament, List<TournamentResult> results)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentId = con.QueryFirstOrDefault<int>(INSERT_TOURNAMENT_QUERY, new { TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription });

            foreach (var resultItem in results)
            {
                if (!string.IsNullOrEmpty(resultItem.Decklist) || !string.IsNullOrEmpty(resultItem.Name))
                    con.Execute(INSERT_RESULT_QUERY, new { TournamentId = tournamentId, resultItem.Placement, resultItem.Name, resultItem.Decklist, Status = 1 });
            }

            return tournamentId;
        }

        public List<Tournament> GetAllTournaments()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournamentResults = con.Query<TournamentResult>("SELECT Id, TournamentId, Placement, Name, Decklist FROM TournamentResult");
            var tournaments = con.Query<Tournament>("SELECT Id, TournamentDate, Name AS TournamentName, Description AS TournamentDescription, Status from Tournament");

            foreach (var tournament in tournaments)
            {
                tournament.TournamentResults = tournamentResults.Where(x => x.TournamentId == tournament.Id).ToList();
            }

            return tournaments.ToList();
        }

        public Tournament GetById(int id)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var tournament = con.QueryFirstOrDefault<Tournament>("SELECT Id, TournamentDate, Name AS TournamentName, Description AS TournamentDescription, Status from Tournament WHERE Id = @Id", new { Id = id });

            return tournament;
        }

        public bool UpdateTournament(int id, Tournament tournament)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var changed = con.Execute(UPDATE_TOURNAMENT_QUERY, new { TournamentId = id, TournamentDate = tournament.TournamentDate.ToShortDateString(), tournament.TournamentName, tournament.TournamentDescription, Status = tournament.Status });

            return changed > 0;
        }

        public bool RegisterForTournament(Tournament tournament, TournamentRegistration registration)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();
            con.Open();

            var result = con.Execute(REGISTER_TOURNAMENT_QUERY, registration);
            return result > 0;
        }
    }
}
