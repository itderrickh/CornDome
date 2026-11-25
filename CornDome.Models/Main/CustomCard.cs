using CornDome.Models.Users;

namespace CornDome.Models.Main
{
    public class CustomCard
    {
        public int CardId { get; set; }
        public Guid Id { get; set; }

        public int UserId { get; set; }

        public string Name { get; set; } = string.Empty;
        public int CardType { get; set; }
        public int? Landscape { get; set; }

        public string Ability { get; set; } = string.Empty;

        public int? Cost { get; set; }
        public int? Attack { get; set; }
        public int? Defense { get; set; }

        public string? Author { get; set; }
        public string? Artist { get; set; }

        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Invisible;
        public int? ApprovedBy { get; set; }

        public virtual User User { get; set; }
        public virtual User? Approver { get; set; }
    }

    public enum Visibility
    {
        Visible,
        Limited,
        Invisible
    }

}
