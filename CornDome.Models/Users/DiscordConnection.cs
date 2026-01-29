namespace CornDome.Models.Users
{
    public class DiscordGuild
    {
        public int Id { get; set; }

        public int DiscordConnectionId { get; set; }
        public DiscordConnection DiscordConnection { get; set; }

        public string GuildId { get; set; }
        public string Name { get; set; }
        public string IconHash { get; set; }
        public bool IsOwner { get; set; }
        public long Permissions { get; set; }

        public DateTime SyncedAt { get; set; } = DateTime.UtcNow;

        public string IconUrl =>
            IconHash == null
                ? null
                : $"https://cdn.discordapp.com/icons/{GuildId}/{IconHash}.png";
    }

    public class DiscordConnection
    {
        public int Id { get; set; }

        public string UserId { get; set; }
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
                : $"https://cdn.discordapp.com/avatars/{DiscordUserId}/{AvatarHash}.png";
    }
}
