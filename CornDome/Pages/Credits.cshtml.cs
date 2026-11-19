using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository.Users;

namespace CornDome.Pages
{
    public class CreditsModel(IUserRepository userRepository) : BasePageModel
    {
        public List<User> PatreonSupporters { get; set; }
        public async Task OnGet()
        {
            PatreonSupporters = await userRepository.GetPatreonSupporters(AppConstants.PATREON_ROLES);
        }
    }
}
