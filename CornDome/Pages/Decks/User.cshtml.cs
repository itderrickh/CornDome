using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Decks
{
    public class UserModel(IDeckRepository deckRepository, ICardRepository cardRepository, IUserRepository userRepository) : BasePageModel
    {
        public List<Card> Cards { get; set; }
        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 48;

        public int TotalPages { get; set; }

        public DeckGridViewModel DeckGrid { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public int? UserId { get; set; }
        public string DisplayUsername { get; set; }
        public bool UserNotFound = false;

        public async Task<IActionResult> OnGet()
        {
            var loggedInUserId = -1;
            if (User.Identity.IsAuthenticated)
            {
                var loggedInUser = await GetUser();
                loggedInUserId = loggedInUser.Id;
            }

            if (UserId == null)
            {
                return Page();
            }

            if (PageNumber < 1)
            {
                PageNumber = 1;
            }

            var user = await userRepository.GetUserById(UserId.Value);
            if (user != null)
            {
                DisplayUsername = user.UserName;

                var results = deckRepository.GetPublicUsersDecks(user.Id, PageNumber, PageSize);
                TotalPages = (int)Math.Ceiling(results.count / (double)PageSize);

                if (TotalPages > 0 && PageNumber > TotalPages)
                {
                    PageNumber = TotalPages;
                }

                DeckGrid = new DeckGridViewModel
                {
                    Decks = results.decks,
                    PageNumber = PageNumber,
                    TotalPages = TotalPages,
                    BaseUrl = BaseUrl,
                    Cards = [.. cardRepository.GetAll()],
                    UserId = loggedInUserId
                };

                UserNotFound = results.decks.Count == 0;
            }
            else
            {
                UserNotFound = true;
            }


            return Page();
        }
    }
}
