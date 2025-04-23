using CornDome.Repository;
using CornDome.Repository.Tournaments;
using Microsoft.EntityFrameworkCore;

namespace CornDome
{
    public class ContextFactory(IConfiguration configuration)
    {
        public CardDatabaseContext CreateCardContext(DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString("CardsDb");

            builder.UseSqlite(connectionString);

            return new CardDatabaseContext(builder.Options);
        }

        public TournamentContext CreateTournamentContext(DbContextOptionsBuilder builder)
        {
            var connectionString = configuration.GetConnectionString("TournamentDb");

            builder.UseSqlite(connectionString);

            return new TournamentContext(builder.Options);
        }
    }
}
