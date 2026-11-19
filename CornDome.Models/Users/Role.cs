using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CornDome.Models.Users
{
    [Table("Role")]
    public class Role : IdentityRole<int>
    {
        [JsonIgnore]
        public ICollection<UserRole> UserRoles { get; set; } = [];
    }
}
