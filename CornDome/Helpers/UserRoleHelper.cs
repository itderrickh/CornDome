using System.Security.Claims;

namespace CornDome.Helpers
{
    public class UserRoleHelper
    {
        public bool IsPatreonMember(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            string[] patreonRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_PATREON_COOLGUY,
                AppConstants.ROLE_PATREON_DWEEB,
                AppConstants.ROLE_PATREON_FLOOPER
            ];

            return IsInAnyRole(principal, patreonRoles);
        }

        public bool IsServedAds(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return true;

            string[] noAdsRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_COOL_GUY,
                AppConstants.ROLE_PATREON_COOLGUY,
                AppConstants.ROLE_PATREON_DWEEB,
                AppConstants.ROLE_PATREON_FLOOPER
            ];

            return !IsInAnyRole(principal, noAdsRoles);
        }

        public bool IsCardManager(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            string[] cardManagerRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_CARD_MANAGER
            ];

            return IsInAnyRole(principal, cardManagerRoles);
        }

        public bool IsTournamentManager(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            string[] cardManagerRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_TOURNAMENT_MANAGER
            ];

            return IsInAnyRole(principal, cardManagerRoles);
        }

        public bool IsAdmin(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            return IsInRole(principal, AppConstants.ROLE_ADMIN);
        }

        public bool IsInRole(ClaimsPrincipal principal, string role)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            return principal.IsInRole(role);
        }

        public bool IsInAnyRole(ClaimsPrincipal principal, params string[] roles)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            return roles.Any(role => principal.IsInRole(role));
        }

        public bool IsInAllRoles(ClaimsPrincipal principal, params string[] roles)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            return roles.All(role => principal.IsInRole(role));
        }
    }
}
