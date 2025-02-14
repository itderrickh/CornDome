using Microsoft.Extensions.Configuration;

namespace CornDome.Repository
{
    public class SqliteRepositoryConfig(IConfiguration configuration)
    {
        public string DbPath { get; set; } = configuration["Cards:DatabasePath"] ?? "";
    }
}
