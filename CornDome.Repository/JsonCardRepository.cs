using CornDome.Models;
using System.Text.Json;

namespace CornDome.Repository
{
    [Obsolete]
    public class JsonCardRepository(JsonRepositoryConfig config) : ICardRepository
    {
        private readonly string _dataDirectory = config.DataPath;
        private readonly List<string> _sets = [
            "cp1.json",
            "cp2.json",
            "cp3.json",
            "cp4.json",
            "cp5.json",
            "cp6.json",
            "2v2.json",
            "ftg.json",
            "landscape.json",
            "promo.json",
            "ks1.json",
            "hp1.json"
        ];

        public IEnumerable<Card> GetAll2()
        {
            return _sets.SelectMany(set => GetCardsFromJson(set));
        }

        public CardFullDetails? GetCard(int id)
        {
            throw new NotImplementedException();
        }

        IEnumerable<CardFullDetails> ICardRepository.GetAll()
        {
            throw new NotImplementedException();
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
