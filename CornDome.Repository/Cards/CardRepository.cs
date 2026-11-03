
using CornDome.Models.Cards;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CornDome.Repository.Cards
{
    public interface ICardRepository
    {
        IEnumerable<Card> GetAll();
        Task<int> GetCount();
        Task<IEnumerable<Card>> GetCards(int limit, int offset);
        Card GetCard(int id);
        IEnumerable<Card> GetCardsFromQuery(List<string> query);

        Card AddCard(Card card);
        bool UpdateCard(Card card);
        bool UpdateCardRulings(Card card);
        bool UpdateCardImage(int cardId, string newImagePath, int cardImageTypeId);
        bool AddCardImage(CardImage cardImage);
        bool DeleteCard(int cardId);

        List<CardSet> GetAllSets();
        List<Landscape> GetAllLandscapes();
        List<CardType> GetAllCardTypes();
        List<CardSet> GetSetsByIds(int[] ids);

        bool AddSet(CardSet cardSet);
        bool UpdateSet(CardSet cardSet);
        bool DeleteSet(int setId);

        Card GetCardOfTheDay(DateTime day);
    }

    public class CardRepository(
        CardDatabaseContext context,
        ICardChangeLogger logger) : ICardRepository
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        };

        private IQueryable<Card> CardQuery()
        {
            return context.Cards
                .AsNoTracking()
                .AsSplitQuery()
                .Include(x => x.CardImages)
                .Include(x => x.CardSets)
                .Include(x => x.CardType)
                .Include(x => x.Landscape);
        }

        public IEnumerable<Card> GetAll()
        {
            return CardQuery()
                .OrderBy(x => x.Id)
                .ToList();
        }

        public async Task<int> GetCount()
        {
            return await context.Cards.CountAsync();
        }

        public async Task<IEnumerable<Card>> GetCards(int limit, int offset)
        {
            return await CardQuery()
                .OrderBy(x => x.Id)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();
        }

        public Card GetCard(int id)
        {
            return CardQuery()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Card> GetCardsFromQuery(List<string> query)
        {
            if (query == null || query.Count == 0)
                return [];

            return CardQuery()
                .Where(x => query.Contains(x.Name))
                .ToList();
        }

        #region Card Management

        public Card AddCard(Card card)
        {
            try
            {
                card.Id = 0;

                card.CardSets = ResolveSets(card.CardSets);
                NormalizeCardProperties(card);

                context.Cards.Add(card);
                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully added card: {card.Name}, Card: {Serialize(card)}");

                return GetCard(card.Id);
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error adding card: {card.Name}. Error: {ex}");

                return null;
            }
        }

        public bool UpdateCard(Card card)
        {
            try
            {
                var dbCard = context.Cards
                    .Include(x => x.CardSets)
                    .FirstOrDefault(x => x.Id == card.Id);

                if (dbCard == null)
                    return false;

                var before = Serialize(dbCard);

                dbCard.IsCustomCard = card.IsCustomCard;
                dbCard.Name = card.Name;
                dbCard.TypeId = card.TypeId;
                dbCard.Ability = card.Ability;
                dbCard.LandscapeId = card.LandscapeId;
                dbCard.Cost = card.Cost;
                dbCard.Attack = card.Attack;
                dbCard.Defense = card.Defense;
                dbCard.Rulings = card.Rulings;

                NormalizeCardProperties(dbCard);

                // Replace set associations with the submitted selections.
                dbCard.CardSets?.Clear();

                foreach (var set in ResolveSets(card.CardSets))
                {
                    dbCard.CardSets.Add(set);
                }

                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully updated card: {card.Name}. Before: {before}. After: {Serialize(dbCard)}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error updating card: {card.Id}. Error: {ex}");

                return false;
            }
        }

        public bool UpdateCardRulings(Card card)
        {
            try
            {
                var dbCard = context.Cards
                    .FirstOrDefault(x => x.Id == card.Id);

                if (dbCard == null)
                    return false;

                var before = dbCard.Rulings;

                dbCard.Rulings = card.Rulings;

                context.SaveChanges();

                logger.LogCardChange(
                    $"Updated rulings for card {card.Id}: {before} -> {dbCard.Rulings}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error updating card rulings: {card.Id}. Error: {ex}");

                return false;
            }
        }

        public bool DeleteCard(int cardId)
        {
            try
            {
                var card = context.Cards
                    .FirstOrDefault(x => x.Id == cardId);

                if (card == null)
                    return false;

                context.Cards.Remove(card);
                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully deleted card: {cardId}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error deleting card: {cardId}. Error: {ex}");

                return false;
            }
        }

        #endregion

        #region Card Images

        public bool UpdateCardImage(
            int cardId,
            string newImagePath,
            int cardImageTypeId)
        {
            try
            {
                var cardImage = context.CardImages
                    .FirstOrDefault(x =>
                        x.CardId == cardId &&
                        x.CardImageTypeId == cardImageTypeId);

                if (cardImage == null)
                {
                    logger.LogCardChange(
                        $"Error updating CardImage: Card {cardId}, Type {cardImageTypeId}. Image not found.");

                    return false;
                }

                var oldPath = cardImage.ImageUrl;

                cardImage.ImageUrl = newImagePath;

                context.SaveChanges();

                logger.LogCardChange(
                    $"Updated CardImage: Card {cardId}, Old: {oldPath}, New: {newImagePath}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error updating CardImage: Card {cardId}. Error: {ex}");

                return false;
            }
        }

        public bool AddCardImage(CardImage cardImage)
        {
            try
            {
                // Resolve the existing card to avoid accidentally inserting
                // a detached card through the navigation property.
                cardImage.Card = null;

                context.CardImages.Add(cardImage);
                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully added CardImage: Card {cardImage.CardId}, Url: {cardImage.ImageUrl}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error adding CardImage: {Serialize(cardImage)}. Error: {ex}");

                return false;
            }
        }

        #endregion

        #region Card Sets

        public List<CardSet> GetAllSets()
        {
            return context.CardSets
                .AsNoTracking()
                .OrderBy(x => x.Description)
                .ToList();
        }

        public List<CardSet> GetSetsByIds(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return [];

            return context.CardSets
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }

        public bool AddSet(CardSet cardSet)
        {
            try
            {
                cardSet.Cards = [];

                context.CardSets.Add(cardSet);
                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully added card set: {cardSet.Description}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error adding card set: {Serialize(cardSet)}. Error: {ex.Message}");

                return false;
            }
        }

        public bool UpdateSet(CardSet cardSet)
        {
            try
            {
                var existingSet = context.CardSets
                    .FirstOrDefault(x => x.Id == cardSet.Id);

                if (existingSet == null)
                    return false;

                existingSet.Value = cardSet.Value;
                existingSet.Description = cardSet.Description;
                existingSet.Code = cardSet.Code;

                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully updated card set: {cardSet.Description}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error updating card set: {cardSet.Id}. Error: {ex.Message}");

                return false;
            }
        }

        public bool DeleteSet(int setId)
        {
            try
            {
                var set = context.CardSets
                    .FirstOrDefault(x => x.Id == setId);

                if (set == null)
                    return false;

                context.CardSets.Remove(set);
                context.SaveChanges();

                logger.LogCardChange(
                    $"Successfully deleted card set: {set.Description}");

                return true;
            }
            catch (Exception ex)
            {
                logger.LogCardChange(
                    $"Error deleting card set: {setId}. Error: {ex.Message}");

                return false;
            }
        }

        #endregion

        #region Reference Data

        public List<Landscape> GetAllLandscapes()
        {
            return context.Landscapes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToList();
        }

        public List<CardType> GetAllCardTypes()
        {
            return context.CardTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .ToList();
        }

        #endregion

        #region Card Of The Day

        public Card GetCardOfTheDay(DateTime day)
        {
            var cards = CardQuery()
                .Where(x => x.TypeId != (int)CardTypeEnum.Landscape)
                .OrderBy(x => x.Id)
                .ToList();

            if (cards.Count == 0)
                return null;

            int seed = day.Year * 10000 + day.Month * 100 + day.Day;

            return cards[new Random(seed).Next(cards.Count)];
        }

        #endregion

        #region Helpers

        private List<CardSet> ResolveSets(
            IEnumerable<CardSet> sets)
        {
            var ids = sets?
                .Select(x => x.Id)
                .Distinct()
                .ToArray() ?? [];

            if (ids.Length == 0)
                return [];

            return context.CardSets
                .Where(x => ids.Contains(x.Id))
                .ToList();
        }

        private static void NormalizeCardProperties(Card card)
        {
            // Reset properties that don't apply to the selected type.
            // This prevents stale data when changing card types.

            switch ((CardTypeEnum)card.TypeId)
            {
                case CardTypeEnum.Creature:
                    break;
                case CardTypeEnum.Spell:
                case CardTypeEnum.Building:
                case CardTypeEnum.Teamwork:
                    card.Attack = null;
                    card.Defense = null;
                    break;
                case CardTypeEnum.Hero:
                    card.Cost = null;
                    card.Attack = null;
                    card.Defense = null;
                    card.LandscapeId = null;
                    break;
                case CardTypeEnum.Landscape:
                    card.CardSets = null;
                    break;
                default:
                    break;
            }
        }

        private static string Serialize(object value)
        {
            return JsonSerializer.Serialize(value, JsonOptions);
        }

        #endregion
    }
}
