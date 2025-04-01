using CornDome.Repository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CornDome.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private IUserRepository _userRepository { get; set; }

        public PermissionHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // Get the user's email or 'sub' from claims
            var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(userEmail))
            {
                userEmail = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value; // Fallback to 'sub' (Google ID)
            }

            if (!string.IsNullOrEmpty(userEmail))
            {
                // Check the permissions from the database
                var userPermission = _userRepository.GetUserPermission(userEmail);

                if (userPermission != null && userPermission.PermissionLevel >= requirement.RequiredPermissionLevel)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail(); // No user found
            }
        }
    }
}
