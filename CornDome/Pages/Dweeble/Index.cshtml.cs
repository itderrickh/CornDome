using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Models.Main.DailyCard;
using CornDome.Repository.Cards;
using CornDome.Repository.Main;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CornDome.Pages.Dweeble
{
    [IgnoreAntiforgeryToken]
    public class IndexModel(IGameRepository gameRepository, ICardRepository cardRepository, ILogEntryRepository logRepository) : BasePageModel
    {
        private const int MaxMistakes = 7;
        private string CheaterCheckCookie = "CardDweebCC";
        private string GuestCookie = "CardDweebDailyGuest";

        public void OnGet()
        {
            
        }

        private bool IsSuspectedCheater()
        {
            var hasCookie = Request.Cookies.TryGetValue(CheaterCheckCookie, out var cookie);
            if (hasCookie && !string.IsNullOrEmpty(cookie))
            {
                return cookie == "true";
            }
            else
            {
                Response.Cookies.Append(
                    CheaterCheckCookie,
                    "false",
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
                    }
                );
                return false;
            }
        }

        private async Task<string> GetPlayerKeyAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var hasGuestCookie = Request.Cookies.TryGetValue(GuestCookie, out var guestId);
            var hasGuestId = Guid.TryParse(guestId, out var parsedGuestId);

            // Authenticated user
            if (User.Identity?.IsAuthenticated == true &&
                !string.IsNullOrWhiteSpace(userId))
            {
                var userKey = $"user:{userId}";

                // If they have a guest cookie, attempt to migrate their game.
                if (hasGuestCookie && hasGuestId)
                {
                    var guestKey = $"guest:{parsedGuestId}";

                    var userGame = await gameRepository.GetDailyCardGameAsync(userKey);

                    if (userGame == null)
                    {
                        var guestGame = await gameRepository.GetDailyCardGameAsync(guestKey);

                        if (guestGame != null)
                        {
                            guestGame.PlayerKey = userKey;

                            // You'll need a repository method to persist this change.
                            await gameRepository.MigrateGuestGame(guestGame, userKey);
                        }
                    }

                    // Remove the guest cookie after processing.
                    Response.Cookies.Delete(GuestCookie);
                }

                return userKey;
            }

            // Anonymous guest
            if (hasGuestCookie)
            {
                return $"guest:{parsedGuestId}";
            }

            // Create a new guest ID
            var newGuestId = Guid.NewGuid().ToString();

            Response.Cookies.Append(
                GuestCookie,
                newGuestId,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax,
                    IsEssential = true,
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                }
            );

            return $"guest:{newGuestId}";
        }

        private async Task<DailyCardGame> GetGameAsync()
        {
            var dailyCard = await gameRepository.GetTodayCardAsync();

            var playerKey = await GetPlayerKeyAsync();

            var game = await gameRepository.GetDailyCardGameAsync(playerKey);

            if (game != null)
                return game;

            game = new DailyCardGame
            {
                DailyCardId = dailyCard.Id,
                PlayerKey = playerKey,
                Started = DateTime.UtcNow
            };

            await gameRepository.AddGame(game, playerKey);

            return game;
        }

        public async Task<IActionResult> OnGetStateAsync()
        {
            var game = await GetGameAsync();

            var dailyCard = await gameRepository.GetTodayCardAsync();

            var response = await BuildResponse(game, dailyCard);
            return new JsonResult(response);
        }

        public async Task<IActionResult> OnPostGuessFullAsync([FromBody] GuessRequest request)
        {
            var game = await GetGameAsync();
            var dailyCard = await gameRepository.GetTodayCardAsync();
            var card = cardRepository.GetCard(dailyCard.CardId);

            var guess = request.Value?.Trim().ToUpperInvariant();

            if (guess == null || guess.Length != card.Name.Length)
            {
                return BadRequest(new { error = "Invalid guess." });
            }

            if (game.IsComplete)
                return BadRequest(new { error = "Game already completed." });


            await gameRepository.UpdateGameNameGuess(game, card, guess, MaxMistakes);

            var response = await BuildResponse(game, dailyCard);
            return new JsonResult(response);
        }

        public async Task<IActionResult> OnPostGuessLetterAsync([FromBody] GuessRequest request)
        {
            var letter = request.Value?.Trim().ToUpperInvariant();

            if (letter == null ||
                letter.Length != 1)
            {
                return BadRequest(new { error = "Invalid letter." });
            }

            var game = await GetGameAsync();

            if (game.IsComplete)
                return BadRequest(new { error = "Game already completed." });

            if (game.GuessedLetters.Contains(letter))
                return BadRequest(new { error = "Letter already guessed." });

            var dailyCard = await gameRepository.GetTodayCardAsync();

            var card = cardRepository.GetCard(game.DailyCard.CardId);
            await gameRepository.UpdateGameLetterGuess(game, card, letter, MaxMistakes);

            var response = await BuildResponse(game, dailyCard);
            return new JsonResult(response);
        }

        public async Task<IActionResult> OnPostGuessAttributeAsync([FromBody] GuessRequest request)
        {
            var key = request.Key?.Trim().ToLowerInvariant();
            var value = request.Value?.Trim();

            var allowed = new[]
            {
                "landscape",
                "cost",
                "attack",
                "defense",
                "type"
            };

            if (key == null ||
                !allowed.Contains(key) ||
                string.IsNullOrWhiteSpace(value))
            {
                return BadRequest(new { error = "Invalid guess." });
            }

            var game = await GetGameAsync();

            if (game.IsComplete)
                return BadRequest(new { error = "Game already completed." });

            if (game.GetAttributes().Contains(key))
                return BadRequest(new { error = "Attribute already guessed." });

            var dailyCard = await gameRepository.GetTodayCardAsync();
            var card = cardRepository.GetCard(dailyCard.CardId);

            var answer = key switch
            {
                "landscape" => card.Landscape?.Value ?? "None",
                "cost" => card.Cost?.ToString() ?? "None",
                "attack" => card.Attack?.ToString() ?? "None",
                "defense" => card.Defense?.ToString() ?? "None",
                "type" => card.CardType?.Value ?? "None",
                _ => string.Empty
            };

            await gameRepository.UpdateGameAttributeGuess(game, card, value, answer, key, MaxMistakes);

            var response = await BuildResponse(game, dailyCard);
            return new JsonResult(response);
        }

        private static bool IsInCharacters(char s)
        {
            return "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".Contains(s.ToString(), StringComparison.CurrentCultureIgnoreCase);
        }

        private async Task<object> BuildResponse(DailyCardGame game, DailyCard dailyCard)
        {
            var card = cardRepository.GetCard(dailyCard.CardId);

            var attributes = game.GetAttributes();

            var nameComplete = game.IsNameComplete(card.Name);

            var revealed = game.IsComplete;

            var namePattern = new string(
                card.Name.Select(c =>
                    !IsInCharacters(c) ||
                    game.GuessedLetters.Contains(
                        char.ToUpperInvariant(c))
                        ? c
                        : '_'
                ).ToArray()
            );

            var abilityPattern = new string(
                card.Ability.Select(c =>
                    !IsInCharacters(c) ||
                    game.GuessedLetters.Contains(
                        char.ToUpperInvariant(c))
                        ? c
                        : '_'
                ).ToArray()
            );

            var cheaterAlert = !game.IsComplete && IsSuspectedCheater();
            if (cheaterAlert)
            {
                await logRepository.InsertAsync(new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Method = "AntiCheat",
                    Path = "/Dweeble",
                    QueryString = "",
                    StatusCode = StatusCodes.Status418ImATeapot,
                    DurationMs = 0,
                    UserName = game.PlayerKey,
                    RemoteIp = HttpContext.Connection.RemoteIpAddress?.ToString(),
                    UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                    Exception = "Possible cheater detected."
                });

                Response.Cookies.Append(
                    "CardDweebCC",
                    "false",
                    new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = Request.IsHttps,
                        SameSite = SameSiteMode.Lax,
                        IsEssential = true,
                        Expires = DateTimeOffset.UtcNow.AddDays(1)
                    }
                );
            }

            return new
            {
                date = dailyCard.Date.ToString("yyyy-MM-dd"),
                namePattern = game.IsComplete ? card.Name : namePattern,
                abilityPattern = nameComplete ? card.Ability : abilityPattern,
                guessedLetters = game.GuessedLetters,
                mistakes = game.Mistakes,
                maxMistakes = MaxMistakes,
                nameComplete,
                isComplete = game.IsComplete,
                won = game.IsComplete && game.Won,
                cardImage = game.IsComplete ? (BaseUrl + card.GetOptimizedImage) : "",
                landscape = attributes.Contains("landscape") || revealed
                    ? card.Landscape?.Value ?? "None"
                    : null,

                cost = attributes.Contains("cost") || revealed
                    ? card.Cost?.ToString() ?? "None"
                    : null,

                attack = attributes.Contains("attack") || revealed
                    ? card.Attack?.ToString() ?? "None"
                    : null,

                defense = attributes.Contains("defense") || revealed
                    ? card.Defense?.ToString() ?? "None"
                    : null,

                type = attributes.Contains("type") || revealed
                    ? card.CardType?.Value ?? "None"
                    : null,

                // Only expose the actual card name after the game ends.
                cardName = revealed ? card.Name : null,
            };
        }

        public class GuessRequest
        {
            public string? Key { get; set; }
            public string? Value { get; set; }
        }
    }
}
