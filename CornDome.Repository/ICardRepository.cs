using CornDome.Models;

namespace CornDome.Repository
{
    public interface ICardRepository
    {
        IEnumerable<CardFullDetails> GetAll();
        CardFullDetails? GetCard(int id);
        IEnumerable<CardFullDetails> GetCardsFromQuery(List<string> query);
    }
}
