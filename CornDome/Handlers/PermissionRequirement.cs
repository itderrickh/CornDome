using Microsoft.AspNetCore.Authorization;

namespace CornDome.Handlers
{
    public class PermissionRequirement(int requiredPermissionLevel) : IAuthorizationRequirement
    {
        public int RequiredPermissionLevel { get; } = requiredPermissionLevel;
    }
}
