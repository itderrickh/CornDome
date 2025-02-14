using CornDome.Models;

namespace CornDome.Repository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAll();
    }
}
