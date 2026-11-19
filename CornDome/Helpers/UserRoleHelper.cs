using CornDome.Models.Users;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CornDome.Helpers
{
    public class UserRoleHelper(UserManager<User> userManager)
    {
        public async Task<bool> IsInRoleAsync(User user, string role)
        {
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> IsInRoleAsync(ClaimsPrincipal principal, string role)
        {
            var user = await userManager.GetUserAsync(principal);
            return user != null && await userManager.IsInRoleAsync(user, role);
        }

        // Returns true if the user has at least one of the specified roles.
        public async Task<bool> IsInAnyRoleAsync(User user, params string[] roles)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            return roles.Any(role => userRoles.Contains(role));
        }

        // Returns true if the user has every specified role.
        public async Task<bool> IsInAllRolesAsync(User user, params string[] roles)
        {
            var userRoles = await userManager.GetRolesAsync(user);
            return roles.All(role => userRoles.Contains(role));
        }

        // ClaimsPrincipal versions.
        public async Task<bool> IsInAnyRoleAsync(ClaimsPrincipal principal, params string[] roles)
        {
            var user = await userManager.GetUserAsync(principal);
            return user != null && await IsInAnyRoleAsync(user, roles);
        }

        public async Task<bool> IsInAllRolesAsync(ClaimsPrincipal principal, params string[] roles)
        {
            var user = await userManager.GetUserAsync(principal);
            return user != null && await IsInAllRolesAsync(user, roles);
        }
    }
}
