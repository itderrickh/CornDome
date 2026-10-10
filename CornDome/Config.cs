using CornDome.Repository.Discord;
using System.Diagnostics;
using System.Reflection;

namespace CornDome
{
    public static class ConfigConstants
    {
        public const string UPLOADPATH = "Cards:Uploads";
        public const string IMAGEPATH = "Cards:Images";
        public const string CARD_CHANGE_LOG_PATH = "Cards:CardModifiedLogs";

        public const string DBPATH_CARDS = "DatabasePaths:CardsDb";
        public const string DBPATH_MASTER = "DatabasePaths:MasterDb";
        public const string DBPATH_TOURNAMENT = "DatabasePaths:TournamentDb";
        public const string DBPATH_BACKUP_LOCATION = "DatabasePaths:BackupLocation";

        public const string BRANDING_TITLE = "Branding:Title";
        public const string CONTENT_STORE_IMAGES = "ContentStore:Images";

        public const string DISCORD_SECRET_PATH = "Authentication:Discord:ClientSecret";
        public const string DISCORD_CLIENTID_PATH = "Authentication:Discord:ClientId";
        public const string DISCORD_GUILDID_PATH = "Authentication:Discord:GuildId";

        public const string GOOGLE_CLIENTID_PATH = "Authentication:Google:ClientId";
        public const string GOOGLE_CLIENTSECRET_PATH = "Authentication:Google:ClientSecret";

        public const string GOOGLE_CLIENTID_ENV_PATH = "Authentication:Google:ClientId";
        public const string GOOGLE_CLIENTSECRET_ENV_PATH = "Authentication:Google:ClientSecret";

        public const string IMAGE_URL_PATH = "/CardImages/";
    }

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
        public string Images { get; set; }
    }

    public class Config
    {
        public Branding Branding { get; set; }
        public AppData AppData { get; set; }
        public ContentStore ContentStore { get; set; }
        public DatabasePaths DatabasePaths { get; set; }
        public DiscordConfiguration DiscordClient { get; set; }
        public string BaseUrl { get
            {
                return ConfigConstants.IMAGE_URL_PATH;
            } }
        public string Version { get; set; }

        public Config(IConfiguration configuration)
        {
            AppData = new AppData()
            {
                UploadPath = configuration[ConfigConstants.UPLOADPATH],
                ImagePath = configuration[ConfigConstants.IMAGEPATH],
                CardModifiedLogs = configuration[ConfigConstants.CARD_CHANGE_LOG_PATH]
            };
            DatabasePaths = new DatabasePaths()
            {
                CardsDb = configuration[ConfigConstants.DBPATH_CARDS],
                MasterDb = configuration[ConfigConstants.DBPATH_MASTER],
                TournamentDb = configuration[ConfigConstants.DBPATH_TOURNAMENT],
                BackupLocation = configuration[ConfigConstants.DBPATH_BACKUP_LOCATION],
            };
            Branding = new Branding() { Title = configuration[ConfigConstants.BRANDING_TITLE] };
            ContentStore = new ContentStore() { Images = configuration[ConfigConstants.CONTENT_STORE_IMAGES] };
            DiscordClient = new DiscordConfiguration()
            {
                ClientId = configuration[ConfigConstants.DISCORD_CLIENTID_PATH],
                ClientSecret = configuration[ConfigConstants.DISCORD_SECRET_PATH],
                GuildId = configuration[ConfigConstants.DISCORD_GUILDID_PATH],
            };
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.ProductVersion;
        }
    }
}
