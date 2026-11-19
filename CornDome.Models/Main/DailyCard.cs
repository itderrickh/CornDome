using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Main.DailyCard;

[Table("dailyCard")]
public class DailyCard
{
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int CardId { get; set; }

    public ICollection<DailyCardGame> Games { get; set; } = [];
}