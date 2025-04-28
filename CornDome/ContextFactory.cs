using CornDome.Repository;
using CornDome.Repository.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace CornDome
{
    public class ContextFactory(IConfiguration configuration)
    {
        public void CreateCardContext(DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString("CardsDb");

            builder.UseSqlite(connectionString);
        }

        public void CreateTournamentContext(DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString("TournamentDb");

            builder.UseSqlite(connectionString);
        }
    }
}
