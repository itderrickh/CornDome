namespace CornDome
{
    public class AppConstants
    {
        public const string APP_NAME = "CardDweeb";

        public const string MASTER_DB = "MasterDb";
        public const string CARDS_DB = "CardsDb";
        public const string TOURNAMENT_DB = "TournamentDb";

        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_TOURNAMENT_MANAGER = "TournamentManager";
        public const string ROLE_CARD_MANAGER = "CardManager";
        public const string ROLE_RULING_MANAGER = "RulingManager";
        public const string ROLE_COOL_GUY = "CoolGuy";
        public const string ROLE_EARLY_BACKER = "Early_Backer";

        public const string ROLE_PATREON_DWEEB = "The_Dweeb";
        public const string ROLE_PATREON_FLOOPER = "The_Flooper";
        public const string ROLE_PATREON_COOLGUY = "The_CoolGuy";

        public const string LOCALHOST = "127.0.0.1";
        public const string DISCORD_TOKEN_ROUTE = "https://discord.com/api/oauth2/token";
        public const string CARD_STATIC_IMAGE_ROUTE = "/CardImages";
        public static string[] PATREON_ROLES = [
            ROLE_PATREON_COOLGUY,
            ROLE_PATREON_DWEEB,
            ROLE_PATREON_FLOOPER
        ];
    }
}
