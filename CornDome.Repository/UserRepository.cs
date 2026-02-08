using CornDome.Models.Users;

namespace CornDome.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<bool> UpdateUser(User user);
        Task<bool> CreateUser(User user);
        Task<User> GetUserById(int id);
        Task<User> GetUserByUsername(string username);
        Task<IEnumerable<User>> GetAll();
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
    }
}
