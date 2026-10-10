using Microsoft.EntityFrameworkCore;

namespace CornDome.Repository.Main
{
    public interface IAppFlagRepository
    {
        List<AppFlag> GetAll();
        void Update(AppFlag flag);
        bool IsActive(string name);
    }

    public class AppFlagRepository(MainContext context) : IAppFlagRepository
    {
        public List<AppFlag> GetAll()
        {
            return context.AppFlags
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ToList();
        }

        public void Update(AppFlag flag)
        {
            var existing = context.AppFlags
                .FirstOrDefault(x => x.Id == flag.Id);

            if (existing == null)
                throw new KeyNotFoundException(
                    $"AppFlag with ID {flag.Id} was not found.");

            existing.Active = flag.Active;

            context.SaveChanges();
        }

        public bool IsActive(string name)
        {
            return context.AppFlags
                .Any(x => x.Name == name && x.Active);
        }
    }
}
