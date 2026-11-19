using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CornDome.Models.Users
{
    [Table("UserRole")]
    public class UserRole
    {
        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
