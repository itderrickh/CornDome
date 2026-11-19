using CornDome.Models.Users;

namespace CornDome.Helpers
{
    public enum UserIcon
    {
        None, 
        Dweeb,
        CoolGuy
    }
    public class IconHelper
    {
        private static readonly string[] DweebRoles = [AppConstants.ROLE_PATREON_DWEEB, AppConstants.ROLE_PATREON_FLOOPER];
        private static readonly string[] CoolGuyRoles = [AppConstants.ROLE_COOL_GUY, AppConstants.ROLE_PATREON_COOLGUY];
        public static UserIcon GetUserIcon(User user)
        {
            if (user.UserRoles.Any(x => DweebRoles.Contains(x.Role.Name)))
            {
                return UserIcon.Dweeb;
            }
            else if (user.UserRoles.Any(x => CoolGuyRoles.Contains(x.Role.Name)))
            {
                return UserIcon.CoolGuy;
            }
            else
            {
                return UserIcon.None;
            }
        }

        public static bool HasIcon(User user)
        {
            return user.UserRoles.Any(x => CoolGuyRoles.Contains(x.Role.Name) || DweebRoles.Contains(x.Role.Name));
        }
    }
}
