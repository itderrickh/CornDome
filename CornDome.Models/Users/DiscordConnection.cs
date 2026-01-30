namespace CornDome.Models.Users
{
    public class DiscordConnection
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
        public string DiscordUserId { get; set; }

        public string Username { get; set; }
        public string Discriminator { get; set; }
        public string AvatarHash { get; set; }

        public string EncryptedAccessToken { get; set; }
        public string EncryptedRefreshToken { get; set; }
        public DateTime TokenExpiresAt { get; set; }

        public DateTime ConnectedAt { get; set; } = DateTime.UtcNow;

        public string AvatarUrl =>
            AvatarHash == null
                ? null
                : $"https://cdn.discordapp.com/avatars/{DiscordUserId}/{AvatarHash}.png?size=60";
    }
}
