using CornDome.Repository;
using System.Data;
using System.Data.SQLite;

namespace CornDome
{
    public class DbConnectionFactory(IConfiguration configuration) : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration = configuration;

        public IDbConnection CreateCardDbConnection()
        {
            return CreateConnection("CardsDb");
        }

        public IDbConnection CreateMasterDbConnection()
        {
            return CreateConnection("MasterDb");
        }

        private IDbConnection CreateConnection(string connectionName)
        {
            var connectionString = _configuration.GetConnectionString(connectionName);
            return new SQLiteConnection(connectionString);
        }
    }
}
