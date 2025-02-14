using Microsoft.Extensions.Configuration;

namespace CornDome.Repository
{
    public class JsonRepositoryConfig(IConfiguration configuration)
    {
        public string DataPath { get; set; } = configuration["Cards:Data"] ?? "";
    }
}
