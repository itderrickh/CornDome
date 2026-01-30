using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Users
{
    [Table("User")]
    public class User : IdentityUser<int>
    {
        public ICollection<UserRole> UserRoles { get; set; } = [];
        public ICollection<DiscordConnection> DiscordConnections { get; set; } = [];
        public ICollection<PlayAvailability> PlayAvailabilities { get; set; } = [];
    }
}
