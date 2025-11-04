using CornDome.Models;
using CornDome.Models.Users;
using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace CornDome.Pages.Admin
{
    [Authorize(Policy = "admin")]
    public class IndexModel(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository) : PageModel
    {
        public User LoggedInUser { get; set; }
        public List<User> Users { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Role> Roles { get; set; }
        [BindProperty]
        public int AddRoleId { get; set; }
        [BindProperty]
        public int AddRoleUserId { get; set; }

        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                LoggedInUser = await userRepository.GetUserByEmail(userEmail);
            }

            await PopulateTables();
        }

        private async Task PopulateTables()
        {
            var users = await userRepository.GetAll();
            Users = users.ToList();
            var userRoles = await userRoleRepository.GetAll();
            UserRoles = userRoles.ToList();
            var roles = await roleRepository.GetAll();
            Roles = roles.ToList();
        }

        public async Task<IActionResult> OnPostAddRole()
        {
            await PopulateTables();

            if (AddRoleId > 0 && AddRoleUserId > 0)
            {
                var user = Users.FirstOrDefault(x => x.Id == AddRoleUserId);
                var role = Roles.FirstOrDefault(x => x.Id == AddRoleId);
                if (user != null && role != null)
                {
                    var isSuccess = await userRoleRepository.AddToRole(user, role);

                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Add role succeeded";
                        await PopulateTables();
                        return Page();
                    }
                }
            }

            TempData["ErrorMessage"] = "Add role failed";
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveRole()
        {
            await PopulateTables();

            if (AddRoleId > 0 && AddRoleUserId > 0)
            {
                var user = Users.FirstOrDefault(x => x.Id == AddRoleUserId);
                var role = Roles.FirstOrDefault(x => x.Id == AddRoleId);
                if (user != null && role != null)
                {
                    var isSuccess = await userRoleRepository.RemoveFromRole(user, role);

                    if (isSuccess)
                    {
                        TempData["SuccessMessage"] = "Remove role succeeded";
                        await PopulateTables();
                        return Page();
                    }
                }
            }

            TempData["ErrorMessage"] = "Remove role failed";
            return Page();
        }
    }
}
