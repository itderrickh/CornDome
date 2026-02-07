using CornDome.Repository.Discord;
using System.Diagnostics;
using System.Reflection;

namespace CornDome
{
    public class Branding
    {
        public string Title { get; set; }
    }

    public class DatabasePaths
    {
        public string CardsDb { get;set; }
        public string MasterDb { get; set; }
        public string TournamentDb { get; set; }
        public string BackupLocation { get; set; }
    }

    public class AppData
    {
        public string ImagePath { get; set; }
        public string UploadPath { get; set; }
        public string CardModifiedLogs { get; set; }
    }

    public class ContentStore
    {
        public string Articles { get; set; }
        public string Images { get; set; }
    }

    public class CloudflareConfig
    {
        public string BucketName { get; set; }
        public string AccountId { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string ServiceUrl { get; set; }
        public string WorkerUrl { get; set; }
    }

    public class Config
    {
        public Branding Branding { get; set; }
        public AppData AppData { get; set; }
        public ContentStore ContentStore { get; set; }
        public DatabasePaths DatabasePaths { get; set; }
        public DiscordConfiguration DiscordClient { get; set; }
        public CloudflareConfig CloudflareConfig { get; set; }
        public string Version { get; set; }

        public Config(IConfiguration configuration)
        {
            AppData = new AppData()
            {
                UploadPath = configuration["Cards:Uploads"],
                ImagePath = configuration["Cards:Images"],
                CardModifiedLogs = configuration["Cards:CardModifiedLogs"]
            };
            DatabasePaths = new DatabasePaths()
            {
                CardsDb = configuration["DatabasePaths:CardsDb"],
                MasterDb = configuration["DatabasePaths:MasterDb"],
                TournamentDb = configuration["DatabasePaths:TournamentDb"],
                BackupLocation = configuration["DatabasePaths:BackupLocation"],
            };
            Branding = new Branding() { Title = configuration["Branding:Title"] };
            ContentStore = new ContentStore() { Articles = configuration["ContentStore:Articles"], Images = configuration["ContentStore:Images"] };
            DiscordClient = new DiscordConfiguration()
            {
                ClientId = configuration["Authentication:Discord:ClientId"],
                ClientSecret = configuration["Authentication:Discord:ClientSecret"],
                GuildId = configuration["Authentication:Discord:GuildId"],
            };
            CloudflareConfig = new CloudflareConfig()
            {
                AccessKey = configuration["Cloudflare:AccessKey"],
                BucketName = configuration["Cloudflare:BucketName"],
                SecretKey = configuration["Cloudflare:SecretKey"],
                AccountId = configuration["Cloudflare:AccountId"],
                ServiceUrl = configuration["Cloudflare:ServiceUrl"],
                WorkerUrl = configuration["Cloudflare:WorkerUrl"]
            };
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.ProductVersion;
        }
    }
}
