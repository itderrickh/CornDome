using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore;
using System.Net.Mime;

namespace CornDome.Repository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAll();
        Card? GetCard(int id);
        IEnumerable<Card> GetCardsFromQuery(List<string> query);
        bool AddCard(Card card, CardRevision cardRevision, CardImage cardImage);
    }

    public class CardRepository(CardDatabaseContext context) : ICardRepository
    {
        public IEnumerable<Card> GetAll()
        {
            return context.Cards
                .Include(card => card.Revisions)
                .ThenInclude(cardRev => cardRev.CardImages)
                .ToList();
        }

        public Card? GetCard(int id)
        {
            return context.Cards
                .Include(card => card.Revisions)
                .ThenInclude(cardRev => cardRev.CardImages)
                .Include(card => card.Revisions)
                .ThenInclude(cardRev => cardRev.CardSet)
                .Include(card => card.Revisions)
                .ThenInclude(cardRev => cardRev.Landscape)
                .Include(card => card.Revisions)
                .ThenInclude(cardRev => cardRev.CardType)
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Card> GetCardsFromQuery(List<string> query)
        {
            var cardsWithLatestRevisions = context.Cards
                .Include(card => card.Revisions)
                .ToList();

            var filtered = cardsWithLatestRevisions
                .Where(card => query.Contains(card.LatestRevision.Name));

            return filtered;
        }

        public bool AddCard(Card card, CardRevision cardRevision, CardImage cardImage)
        {
            var addSuccess = false;
            using var transcation = context.Database.BeginTransaction();

            try
            {
                context.Add(card);
                context.SaveChanges();

                cardRevision.CardId = card.Id;
                context.Add(cardRevision);
                var revisionId = context.SaveChanges();

                cardImage.RevisionId = cardRevision.Id;
                context.Add(cardImage);
                context.SaveChanges();

                transcation.Commit();
                addSuccess = true;
            }
            catch (Exception e)
            {
                transcation.Rollback();
            }

            return addSuccess;
        }
    }
}
