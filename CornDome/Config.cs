using CornDome.Repository.Discord;
using System.Diagnostics;
using System.Reflection;

namespace CornDome
{
    public static class ConfigConstants
    {
        public const string UPLOADPATH = "Cards:Uploads";
        public const string IMAGEPATH = "Cards:Images";
        public const string CUSTOMPATH = "Cards:Custom";
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
        public const string CUSTOM_IMAGE_URL = "/api/customcards/{id}/image";

        public const string PATREON_CLIENTID = "Patreon:ClientId";
        public const string PATREON_CLIENTSECRET = "Patreon:ClientSecret";
        public const string PATREON_REDIRECT_URI = "Patreon:RedirectUri";
        public const string PATREON_CAMPAIGNID = "Patreon:CampaignId";
        public const string PATREON_TIERS_DWEEB = "Patreon:TierRoles:TheDweeb";
        public const string PATREON_TIERS_FLOOPER = "Patreon:TierRoles:TheFLooper";
        public const string PATREON_TIERS_COOLGUY = "Patreon:TierRoles:TheCoolGuy";
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
        public string CustomPath { get; set; }
        public string CardModifiedLogs { get; set; }
    }

    public class ContentStore
    {
        public string Images { get; set; }
    }

    public class PatreonConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string CampaignId { get; set; }
        public TierRoles TierRoles { get; set; }
    }

    public class TierRoles
    {
        public string TheDweeb { get; set; }
        public string TheFlooper { get; set; }
        public string TheCoolGuy { get; set; }
    }

    public class Config
    {
        public Branding Branding { get; set; }
        public AppData AppData { get; set; }
        public ContentStore ContentStore { get; set; }
        public DatabasePaths DatabasePaths { get; set; }
        public DiscordConfiguration DiscordClient { get; set; }
        public PatreonConfig PatreonConfig { get; set; }
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
                CustomPath = configuration[ConfigConstants.CUSTOMPATH],
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
            PatreonConfig = new PatreonConfig()
            {
                ClientId = configuration[ConfigConstants.PATREON_CLIENTID],
                ClientSecret = configuration[ConfigConstants.PATREON_CLIENTSECRET],
                RedirectUri = configuration[ConfigConstants.PATREON_REDIRECT_URI],
                CampaignId = configuration[ConfigConstants.PATREON_CAMPAIGNID],
                TierRoles = new TierRoles()
                {
                    TheCoolGuy = configuration[ConfigConstants.PATREON_TIERS_COOLGUY],
                    TheDweeb = configuration[ConfigConstants.PATREON_TIERS_DWEEB],
                    TheFlooper = configuration[ConfigConstants.PATREON_TIERS_FLOOPER]
                }
            };
            
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            Version = fileVersionInfo.ProductVersion;
        }
    }
}
