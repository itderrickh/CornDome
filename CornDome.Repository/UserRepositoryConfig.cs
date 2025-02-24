using Microsoft.Extensions.Configuration;

namespace CornDome.Repository
{
    public class UserRepositoryConfig(IConfiguration configuration)
    {
        public string DbPath { get; set; } = configuration["Database:MasterPath"] ?? "";
    }
}
