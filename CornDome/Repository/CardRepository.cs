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

        public IEnumerable<Card> GetAll()
        {
            return GetHeroes()
                .Concat(GetBuildings())
                .Concat(GetCreatures())
                .Concat(GetLandscapes())
                .Concat(GetSpells())
                .Concat(GetTeamworks());
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

        private List<Card> GetHeroes() => GetCardsFromJson("hero.json");

        private List<Card> GetCreatures() => GetCardsFromJson("creature.json");

        private List<Card> GetBuildings() => GetCardsFromJson("building.json");

        private List<Card> GetLandscapes() => GetCardsFromJson("landscape.json");

        private List<Card> GetSpells() => GetCardsFromJson("spell.json");

        private List<Card> GetTeamworks() => GetCardsFromJson("teamwork.json");
    }
}
