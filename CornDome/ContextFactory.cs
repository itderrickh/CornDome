using CornDome.Repository;
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
    }
}
