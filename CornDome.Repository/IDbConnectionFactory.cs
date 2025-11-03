using System.Data;

namespace CornDome.Repository
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateMasterDbConnection();
    }
}
