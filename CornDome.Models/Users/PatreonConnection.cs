using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Users
{
    [Table("PatreonConnection")]
    public class PatreonConnection
    {
        [Key]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public string PatreonUserId { get; set; } = "";

        public string? PatreonMemberId { get; set; }

        public string? CurrentTierId { get; set; }

        public string? AccessToken { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime TokenExpiresAt { get; set; }

        public DateTime LastSyncedAt { get; set; }

        public bool IsActive { get; set; }
    }
}
