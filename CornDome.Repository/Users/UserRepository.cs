using CornDome.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Users
{
    public interface IUserRepository
    {
        Task<bool> DeleteUser(int id);
        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<bool> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string username);
        Task<IEnumerable<User>> GetAll();
        Task<bool> LoginSuccess(User user);
        Task<List<User>> GetByIds(int[] ids);
        Task<List<User>> GetPatreonSupporters(string[] patreonRoles);
    }

    public class UserRepository(MainContext context) : IUserRepository
    {
        public async Task<User> GetUserByEmail(string email)
        {
            return context.Users
                .FirstOrDefault(x => x.Email == email);
        }

        public async Task<User> GetUserById(int id)
        {
            return context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(y => y.Role)
                .FirstOrDefault(x => x.Id == id);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return context.Users
                .FirstOrDefault(x => x.UserName == username);
        }

        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var dbUser = context.Users.FirstOrDefault(x => x.Id == user.Id);
                dbUser?.Email = user.Email;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateUser(User user)
        {
            try
            {
                context.Add(user);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetAll()
        {
            return context.Users;
        }

        public async Task<bool> LoginSuccess(User user)
        {
            try
            {
                var dbUser = context.Users.FirstOrDefault(x => x.Id == user.Id);
                dbUser?.LastLogin = DateTime.UtcNow;

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<User>> GetByIds(int[] ids)
        {
            return await context.Users
                .Include(x => x.UserRoles)
                    .ThenInclude(y => y.Role)
                .Where(x => ids.Contains(x.Id))
                .ToListAsync();
        }

        public async Task<List<User>> GetPatreonSupporters(string[] patreonRoles)
        {
            return await context.Users
                .Include(x => x.UserRoles)
                .ThenInclude(y => y.Role)
                .Where(u => u.UserRoles.Any(x => patreonRoles.Contains(x.Role.Name)))
                .ToListAsync();
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var user = await context.Users
                    .Include(x => x.UserRoles)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (user == null)
                    return false;

                // Remove associated user roles.
                context.UserRoles.RemoveRange(user.UserRoles);

                // Remove all user-related records.
                await context.BugReports
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.DeckComments
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.DeckLikes
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.Decks
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.DiscordConnections
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.PatreonConnections
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.PlayAvailabilities
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.PlayPreferences
                    .Where(x => x.UserId == id)
                    .ExecuteDeleteAsync();

                await context.DailyCardGames
                    .Where(x => x.PlayerKey == $"user:{id}")
                    .ExecuteDeleteAsync();


                // Remove the user.
                context.Users.Remove(user);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
