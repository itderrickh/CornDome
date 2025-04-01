namespace CornDome.Models
{
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionLevel { get; set; }
    }
}
