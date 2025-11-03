using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CornDome.Repository
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAll();
        Card? GetCard(int id);
        IEnumerable<Card> GetCardsFromQuery(List<string> query);
        bool AddCard(Card card, CardRevision cardRevision, CardImage cardImage);
        bool UpdateCardAndRevisions(Card card);
    }

    public class CardRepository(CardDatabaseContext context, ICardChangeLogger logger) : ICardRepository
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

        public bool UpdateCardAndRevisions(Card card)
        {
            using var transaction = context.Database.BeginTransaction();

            try
            {
                var dbCard = context.Cards.FirstOrDefault(x => x.Id == card.Id);
                if (dbCard == null)
                {
                    return false;
                }

                dbCard.IsCustomCard = card.IsCustomCard;
                context.SaveChanges();

                foreach (var revision in card.Revisions)
                {
                    var dbRev = context.CardRevisions.FirstOrDefault(x => x.Id == revision.Id);
                    var beforeChanges = "";
                    if (dbRev != null)
                    {
                        beforeChanges = JsonSerializer.Serialize(dbRev);

                        dbRev.Name = revision.Name;
                        dbRev.TypeId = revision.TypeId;

                        if (revision.TypeId == (int)CardTypeEnum.Creature)
                        {
                            dbRev.Ability = revision.Ability;
                            dbRev.SetId = revision.SetId;
                            dbRev.Cost = revision.Cost;
                            dbRev.Attack = revision.Attack;
                            dbRev.Defense = revision.Defense;
                            dbRev.LandscapeId = revision.LandscapeId;
                        }
                        else if (revision.TypeId == (int)CardTypeEnum.Spell)
                        {
                            dbRev.Ability = revision.Ability;
                            dbRev.SetId = revision.SetId;
                            dbRev.Cost = revision.Cost;
                            dbRev.LandscapeId = revision.LandscapeId;
                        }
                        else if (revision.TypeId == (int)CardTypeEnum.Hero)
                        {
                            dbRev.Ability = revision.Ability;
                            dbRev.SetId = revision.SetId;
                        }
                        else if (revision.TypeId == (int)CardTypeEnum.Building)
                        {
                            dbRev.Ability = revision.Ability;
                            dbRev.SetId = revision.SetId;
                            dbRev.Cost = revision.Cost;
                            dbRev.LandscapeId = revision.LandscapeId;
                        }
                        else if (revision.TypeId == (int)CardTypeEnum.Teamwork)
                        {
                            dbRev.Ability = revision.Ability;
                            dbRev.SetId = revision.SetId;
                            dbRev.Cost = revision.Cost;
                            dbRev.LandscapeId = revision.LandscapeId;
                        }
                    }
                    context.SaveChanges();
                    logger.LogCardChange($"Updating card revision: {revision.Name}, Updated: {JsonSerializer.Serialize(revision)}");
                }

                transaction.Commit();
                return true;
            }
            catch (Exception)
            {
                transaction.Rollback();
                logger.LogCardChange($"Error Updating Card: {JsonSerializer.Serialize(card)}");
            }

            return false;
        }

        public bool AddCard(Card card, CardRevision cardRevision, CardImage cardImage)
        {
            var addSuccess = false;
            using var transcation = context.Database.BeginTransaction();

            try
            {
                context.Add(card);
                context.SaveChanges();

                var revisionToInsert = RemoveInvalidProperties(cardRevision);
                revisionToInsert.CardId = card.Id;
                context.Add(revisionToInsert);
                var revisionId = context.SaveChanges();

                cardImage.RevisionId = revisionToInsert.Id;
                context.Add(cardImage);
                context.SaveChanges();

                transcation.Commit();
                addSuccess = true;
                logger.LogCardChange($"Adding card: {cardRevision.Name}, Card: {JsonSerializer.Serialize(card)}, Revision: {JsonSerializer.Serialize(cardRevision)}, Card Image: {JsonSerializer.Serialize(cardImage)}");
            }
            catch (Exception)
            {
                logger.LogCardChange($"Error Adding card: {cardRevision.Name}, Card: {JsonSerializer.Serialize(card)}, Revision: {JsonSerializer.Serialize(cardRevision)}, Card Image: {JsonSerializer.Serialize(cardImage)}");
                transcation.Rollback();
            }

            return addSuccess;
        }

        private static CardRevision RemoveInvalidProperties(CardRevision cardRevision)
        {
            var dbRev = new CardRevision
            {
                Name = cardRevision.Name,
                RevisionNumber = cardRevision.RevisionNumber,
                TypeId = cardRevision.TypeId
            };

            if (cardRevision.TypeId == (int)CardTypeEnum.Creature)
            {
                dbRev.Ability = cardRevision.Ability;
                dbRev.SetId = cardRevision.SetId;
                dbRev.Cost = cardRevision.Cost;
                dbRev.Attack = cardRevision.Attack;
                dbRev.Defense = cardRevision.Defense;
                dbRev.LandscapeId = cardRevision.LandscapeId;
            }
            else if (cardRevision.TypeId == (int)CardTypeEnum.Spell)
            {
                dbRev.Ability = cardRevision.Ability;
                dbRev.SetId = cardRevision.SetId;
                dbRev.Cost = cardRevision.Cost;
                dbRev.LandscapeId = cardRevision.LandscapeId;
            }
            else if (cardRevision.TypeId == (int)CardTypeEnum.Hero)
            {
                dbRev.Ability = cardRevision.Ability;
                dbRev.SetId = cardRevision.SetId;
            }
            else if (cardRevision.TypeId == (int)CardTypeEnum.Building)
            {
                dbRev.Ability = cardRevision.Ability;
                dbRev.SetId = cardRevision.SetId;
                dbRev.Cost = cardRevision.Cost;
                dbRev.LandscapeId = cardRevision.LandscapeId;
            }
            else if (cardRevision.TypeId == (int)CardTypeEnum.Teamwork)
            {
                dbRev.Ability = cardRevision.Ability;
                dbRev.SetId = cardRevision.SetId;
                dbRev.Cost = cardRevision.Cost;
                dbRev.LandscapeId = cardRevision.LandscapeId;
            }

            return dbRev;
        }
    }
}
