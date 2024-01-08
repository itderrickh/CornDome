using CornDome.Models;
using System.Text.Json;

namespace CornDome.Repository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAll();
    }
    public class CardRepository(Config config) : ICardRepository
    {
        private readonly string _dataDirectory = config.AppData.DataPath;
        private readonly List<string> _sets = [
            "cp1.json",
            "cp2.json",
            "cp3.json",
            "cp4.json",
            "cp5.json",
            "cp6.json",
            "dbl.json",
            "ftg.json",
            "landscape.json",
            "promo.json"
        ];

        public IEnumerable<Card> GetAll()
        {
            return _sets.SelectMany(set => GetCardsFromJson(set));
        }

        private List<Card> GetCardsFromJson(string file)
        {
            List<Card> cards = [];
            var data = File.ReadAllText(Path.Combine(_dataDirectory, file));
            if (!string.IsNullOrEmpty(data))
            {
                cards = JsonSerializer.Deserialize<IEnumerable<Card>>(data).ToList();
            }
            return cards;
        }
    }
}
