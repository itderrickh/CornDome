
using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CornDome.Repository.Patreon
{
    public interface IPatreonRoleService
    {
        Task SyncRolesAsync(
            User user,
            IEnumerable<string> entitledTierIds);
    }

    public class PatreonRoleService(MainContext context, IOptions<PatreonOptions> options) : IPatreonRoleService
    {
        private readonly PatreonOptions _options = options.Value;

        public async Task SyncRolesAsync(User user, IEnumerable<string> entitledTierIds)
        {
            var tierRoles = _options.TierRoles;

            var patreonRoleNames = tierRoles.Values
                .Distinct()
                .ToList();

            // Find all configured Patreon roles.
            var roles = await context.Roles
                .Where(x => patreonRoleNames.Contains(x.Name!))
                .ToDictionaryAsync(x => x.Name!, x => x.Id);

            // Ensure every configured Patreon role exists.
            var missingRoles = patreonRoleNames
                .Where(x => !roles.ContainsKey(x))
                .ToList();

            if (missingRoles.Count > 0)
            {
                throw new InvalidOperationException(
                    $"Missing Patreon roles: {string.Join(", ", missingRoles)}");
            }

            // Determine the roles the user should have.
            var desiredRoleIds = entitledTierIds
                .Where(tierRoles.ContainsKey)
                .Select(tierId => roles[tierRoles[tierId]])
                .ToHashSet();

            // Get existing Patreon-managed roles.
            var existingRoles = await context.UserRoles
                .Where(x =>
                    x.UserId == user.Id &&
                    patreonRoleNames.Contains(x.Role.Name!))
                .ToListAsync();

            var existingRoleIds = existingRoles
                .Select(x => x.RoleId)
                .ToHashSet();

            // Remove roles no longer entitled.
            var rolesToRemove = existingRoles
                .Where(x => !desiredRoleIds.Contains(x.RoleId))
                .ToList();

            context.UserRoles.RemoveRange(rolesToRemove);

            // Add newly entitled roles.
            var rolesToAdd = desiredRoleIds
                .Except(existingRoleIds)
                .Select(roleId => new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId
                });

            context.UserRoles.AddRange(rolesToAdd);

            await context.SaveChangesAsync();
        }
    }
}
