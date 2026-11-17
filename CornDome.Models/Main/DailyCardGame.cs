using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace CornDome.Models.Main.DailyCard;

[Table("dailyCardGame")]
public class DailyCardGame
{
    public int Id { get; set; }

    public int DailyCardId { get; set; }

    [MaxLength(100)]
    public string PlayerKey { get; set; } = string.Empty;

    [MaxLength(26)]
    public string GuessedLetters { get; set; } = string.Empty;

    [MaxLength(100)]
    public string GuessedAttributes { get; set; } = string.Empty;

    public int Mistakes { get; set; }

    public bool IsComplete { get; set; }

    public bool Won { get; set; }

    public DateTime Started { get; set; }

    public DateTime? Completed { get; set; }

    public DailyCard DailyCard { get; set; } = null!;

    public bool IsNameComplete(string name)
    {
        return name.All(c =>
            !char.IsLetter(c) ||
            GuessedLetters.Contains(
                char.ToUpperInvariant(c)
            )
        );
    }

    public List<string> GetAttributes()
    {
        return string.IsNullOrWhiteSpace(GuessedAttributes)
            ? []
            : GuessedAttributes.Split(',').ToList();
    }

    public void AddAttribute(string attribute)
    {
        var attributes = GetAttributes();

        if (!attributes.Contains(attribute))
            attributes.Add(attribute);

        GuessedAttributes = string.Join(",", attributes);
    }

    public void FinishGame(bool won)
    {
        IsComplete = true;
        Won = won;
        Completed = DateTime.UtcNow;
    }

    public void CheckWin(string name)
    {
        var required = new[]
        {
                "ability",
                "landscape",
                "cost",
                "attack",
                "defense",
                "type"
            };

        if (IsNameComplete(name))
        {
            FinishGame(true);
        }
    }
}