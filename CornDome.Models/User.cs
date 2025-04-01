using Microsoft.AspNetCore.Identity;

namespace CornDome.Models
{
    public class User : IdentityUser<int>
    {
        public string Username { get; set; }
    }
}
