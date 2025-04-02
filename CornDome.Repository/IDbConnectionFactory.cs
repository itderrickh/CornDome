using System.Data;

namespace CornDome.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateCardDbConnection();
        IDbConnection CreateMasterDbConnection();
    }
}
