using CornDome.Models;
using CornDome.Models.Main;
using CornDome.Models.Users;
using CornDome.Repository.Main;
using CornDome.Repository.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class IndexModel : BasePageModel
    {
        public User LoggedInUser { get; set; }

        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                LoggedInUser = await GetUser();
            }
        }
    }
}
