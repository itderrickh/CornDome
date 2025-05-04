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
    public class IndexModel(IUserRepository userRepository, IFeedbackRepository feedbackRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository) : PageModel
    {
        public User LoggedInUser { get; set; }
        public List<FeedbackRequest> FeedbackRequests { get; set; }
        public List<User> Users { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Role> Roles { get; set; }
        [BindProperty]
        public int DeleteFeedbackId { get; set; }

        public async void OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userEmail = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                LoggedInUser = await userRepository.GetUserByEmail(userEmail);
            }

            FeedbackRequests = feedbackRepository.GetAllFeedback();
            var users = await userRepository.GetAll();
            Users = users.ToList();
            var userRoles = await userRoleRepository.GetAll();
            UserRoles = userRoles.ToList();
            var roles = await roleRepository.GetAll();
            Roles = roles.ToList();
        }

        public IActionResult OnPostDeleteFeedback()
        {
            var result = feedbackRepository.DeleteFeedback(DeleteFeedbackId);

            if (result == 1)
                TempData["SuccessMessage"] = "Delete feedback succeeded";
            else
                TempData["ErrorMessage"] = "Delete feedback failed";

            return RedirectToPage();
        }
    }
}
