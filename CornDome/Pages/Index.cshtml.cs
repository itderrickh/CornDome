using CornDome.Models;
using CornDome.Models.Cards;
using CornDome.Models.Users;
using CornDome.Repository.Cards;
using CornDome.Repository.Users;

namespace CornDome.Pages
{
    public class IndexModel(ICardRepository cardRepository, IUserRepository userRepository) : BasePageModel
    {
        public Card CardOfTheDay { get; set; }
        public List<User> PatreonSupporters { get; set; }

        public async Task OnGet()
        {
            CardOfTheDay = cardRepository.GetCardOfTheDay(DateTime.Now);
            PatreonSupporters = await userRepository.GetPatreonSupporters(AppConstants.PATREON_ROLES);
        }
    }
}
