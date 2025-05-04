using CornDome.Repository.Tournaments;
using CornDome.Models.Tournaments;
using Microsoft.EntityFrameworkCore;
using CornDome.Repository;
using CornDome.Models.Users;

namespace CornDome.TournamentSystem
{
    public class TournamentManager
    {
        private TournamentContext context;
        private IUserRepository userRepo;
        private Tournament Tournament { get; set; }
        private List<User> users;
        private List<Player> Players = [];
        public int CurrentRound = 0;

        public TournamentManager(int tournamentId, TournamentContext tournamentContext, IUserRepository userRepository)
        {
            context = tournamentContext;
            userRepo = userRepository;

            Tournament = context.Tournaments
                .Include(t => t.Rounds)
                .ThenInclude(r => r.Matches)
                .FirstOrDefault(x => x.Id == tournamentId);

            SetupState();
        }

        private async void SetupState()
        {
            var allUsers = await userRepo.GetAll();
            users = allUsers.ToList();

            CurrentRound = Tournament.Rounds.Any() ? Tournament.Rounds.Max(x => x.RoundNumber) : 0;

            CreatePlayerListFromRegistrations();
        }

        private void CreatePlayerListFromRegistrations()
        {
            var userRegistrations = context.Registrations.Where(x => x.TournamentId == Tournament.Id).ToList();

            if (!userRegistrations.Any())
                return;

            foreach (var user in userRegistrations)
            {
                Players.Add(new Player()
                {
                    User = user.User,
                    Stats = new PlayerStats()
                });
            }

            var allMatches = context.Matches.Where(x => x.TournamentId == Tournament.Id);
            foreach (var match in allMatches)
            {
                var player1 = Players.FirstOrDefault(x => x.User.Id == match.Player1Id);
                var player2 = Players.FirstOrDefault(x => x.User.Id == match.Player2Id);

                if (player1 != null)
                {
                    player1.Stats.Played++;
                    player1.Stats.Opponents.Add(player2);
                    if (match.Result == MatchResult.Player1Wins || match.Result == MatchResult.Bye)
                    {
                        player1.Stats.Points += TournamentSettings.POINTS_PER_WIN;
                        player1.Stats.Wins += 1;
                    }                        
                    else if (match.Result == MatchResult.Tie)
                    {
                        player1.Stats.Points += TournamentSettings.POINTS_PER_DRAW;
                        player1.Stats.Ties += 1;
                    }
                    else if (match.Result == MatchResult.Player2Wins)
                    {
                        player1.Stats.Points += TournamentSettings.POINTS_PER_LOSS;
                        player1.Stats.Losses += 1;
                    } 
                }

                if (player2 != null)
                {
                    player2.Stats.Played++;
                    player2.Stats.Opponents.Add(player1);
                    if (match.Result == MatchResult.Player2Wins)
                    {
                        player2.Stats.Points += TournamentSettings.POINTS_PER_WIN;
                        player2.Stats.Wins += 1;
                    }
                    else if (match.Result == MatchResult.Tie)
                    {
                        player2.Stats.Points += TournamentSettings.POINTS_PER_DRAW;
                        player2.Stats.Ties += 1;
                    }
                    else if (match.Result == MatchResult.Player1Wins)
                    {
                        player2.Stats.Points += TournamentSettings.POINTS_PER_LOSS;
                        player2.Stats.Losses += 1;
                    }
                }
            }

            // Calculate OMW (Opponent Match Win %)
            foreach (var player in Players)
            {
                var opponentWinRates = new List<double>();

                foreach (var opponent in player.Stats.Opponents)
                {
                    // Skip if bye case
                    if (opponent != null)
                    {
                        var opponentStats = opponent.Stats;
                        if (opponentStats != null && opponentStats.Played > 0)
                        {
                            double opponentWinRate = opponentStats.Points / opponentStats.Played;
                            opponentWinRates.Add(opponentWinRate);
                        }
                    }
                }

                // If no opponents, OMW is 0
                player.Stats.OMW = opponentWinRates.Count > 0 ? opponentWinRates.Average() : 0;
            }
        }

        public bool AllMatchesCompleted()
        {
            if (CurrentRound == 0)
                return true;

            var round = context.Rounds
                .Include(x => x.Matches)
                .SingleOrDefault(x => x.TournamentId == Tournament.Id && x.RoundNumber == CurrentRound);

            return round?.Matches.All(x => x.Result != MatchResult.Incomplete) ?? false;
        }

        public bool ReportResult(int matchId, MatchResult result)
        {
            var match = context.Matches.FirstOrDefault(x => x.Id == matchId);

            if (match != null)
            {
                match.Result = result;
                return context.SaveChanges() > 0;
            }

            return false;
        }

        public bool PairNextRound()
        {
            if (!AllMatchesCompleted())
                return false;

            var isSuccess = false;
            CurrentRound += 1;

            var roundAdded = context.Rounds.Add(new Round()
            {
                TournamentId = Tournament.Id,
                RoundNumber = CurrentRound,
            });
            var wasRoundAdded = context.SaveChanges() > 0;

            if (wasRoundAdded)
            {
                List<Match> pairings = [];

                var matchHistory = context.Matches.Where(x => x.TournamentId == Tournament.Id);
                var byeHistory = matchHistory.Where(m => m.Player2Id == null).Select(m => m.Player1Id).ToHashSet();

                // Shuffle players and sort by standings
                Random rng = new();
                var sortedStandings = Players.OrderBy(x => rng.Next()).ToList();
                sortedStandings = sortedStandings.OrderByDescending(s => s.Stats.Points).ThenByDescending(s => s.Stats.OMW).ToList();

                List<Player> playerList = new List<Player>(sortedStandings);

                // Handle bye if odd number of players
                if (playerList.Count % 2 != 0)
                {
                    var byeCandidate = playerList.FirstOrDefault(p => !byeHistory.Contains(p.User.Id));
                    if (byeCandidate != null)
                    {
                        pairings.Add(new Match()
                        {
                            Player1Id = byeCandidate.User.Id,
                            Player2Id = null, // null indicates a bye
                            Result = MatchResult.Bye,
                            TournamentId = Tournament.Id,
                            RoundId = roundAdded.Entity.Id
                        });
                        playerList.Remove(byeCandidate);
                    }
                    else
                    {
                        // fallback: give someone another bye if all had one (edge case)
                        var fallbackBye = playerList.First();
                        pairings.Add(new Match()
                        {
                            Player1Id = fallbackBye.User.Id,
                            Player2Id = null,
                            Result = MatchResult.Bye,
                            TournamentId = Tournament.Id,
                            RoundId = roundAdded.Entity.Id
                        });
                        playerList.Remove(fallbackBye);
                    }
                }

                // Pair the remaining players
                while (playerList.Count > 0)
                {
                    Player primaryPlayer = playerList[0];
                    List<Player> previouslyPlayed = [];

                    foreach (var match in matchHistory)
                    {
                        if (match.Player1Id == primaryPlayer.User.Id)
                        {
                            var otherPlayer = sortedStandings.SingleOrDefault(x => x.User.Id == match.Player2Id);
                            if (otherPlayer != null)
                                previouslyPlayed.Add(otherPlayer);
                        }
                        else if (match.Player2Id == primaryPlayer.User.Id)
                        {
                            var otherPlayer = sortedStandings.SingleOrDefault(x => x.User.Id == match.Player1Id);
                            if (otherPlayer != null)
                                previouslyPlayed.Add(otherPlayer);
                        }
                    }

                    for (int i = 1; i < playerList.Count; i++)
                    {
                        Player opponent = playerList[i];
                        if (!previouslyPlayed.Contains(opponent))
                        {
                            pairings.Add(new Match()
                            {
                                Player1Id = primaryPlayer.User.Id,
                                Player2Id = opponent.User.Id,
                                Result = MatchResult.Incomplete,
                                TournamentId = Tournament.Id,
                                RoundId = roundAdded.Entity.Id
                            });
                            playerList.RemoveAt(i);
                            playerList.RemoveAt(0);
                            break;
                        }
                    }

                    // Fallback: no valid opponent found (can happen if everyone already played)
                    if (playerList.Count > 0 && playerList[0] == primaryPlayer)
                    {
                        playerList.RemoveAt(0);
                    }
                }

                context.Matches.AddRange(pairings);
                var wereMatchesAdded = context.SaveChanges() > 0;
                isSuccess = wereMatchesAdded;
            }

            return isSuccess;
        }

        public List<Player> GetStandings() => Players;
        public Round GetRound()
        {
            return context.Rounds
                .Include(x => x.Matches)
                .SingleOrDefault(x => x.RoundNumber == CurrentRound && x.TournamentId == Tournament.Id);
        }

        public bool IsTournamentComplete()
        {
            return CurrentRound == NumberOfRounds() && AllMatchesCompleted();
        }

        public int NumberOfRounds()
        {
            int playerCount = Players.Count;
            var roundCount = (int)Math.Ceiling(Math.Log2(playerCount));
            return roundCount == 0 ? 1 : roundCount;
        }
    }
}
