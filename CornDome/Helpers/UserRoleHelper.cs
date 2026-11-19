using System.Security.Claims;

namespace CornDome.Helpers
{
    public class UserRoleHelper
    {
        public bool IsServedAds(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return true;

            string[] noAdsRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_COOL_GUY
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

        public bool IsRulingManager(ClaimsPrincipal principal)
        {
            if (principal == null || !principal.Identity.IsAuthenticated)
                return false;

            string[] cardManagerRoles = [
                AppConstants.ROLE_ADMIN,
                AppConstants.ROLE_RULING_MANAGER
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
