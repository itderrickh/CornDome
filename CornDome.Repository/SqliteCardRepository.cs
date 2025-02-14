using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public class SqliteCardRepository(SqliteRepositoryConfig config) : ICardRepository
    {

        public IEnumerable<CardFullDetails> GetAll()
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var cardRevisions = con.Query<CardRevision>("SELECT id, revisionNumber, cardId, name, typeId as cardType, ability, landscapeId as landscape, cost, attack, defense, setId as cardset FROM card_revision");

            var revisionIds = cardRevisions.Select(x => x.Id).ToList();
            var cardImages = con.Query<CardImage>("SELECT id, revisionId, cardImageTypeId AS cardImageType, imageUrl FROM card_image WHERE revisionId IN @RevisionIds", new { RevisionIds = revisionIds });


            foreach (var cardImage in cardImages)
            {
                if (cardRevisions.Any(x => x.Id == cardImage.RevisionId))
                {
                    cardRevisions.FirstOrDefault(x => x.Id == cardImage.RevisionId).CardImages.Add(cardImage);
                }
            }

            Dictionary<int, CardFullDetails> data = [];
            foreach (var cardRevision in cardRevisions)
            {
                if (data.TryGetValue(cardRevision.CardId, out CardFullDetails? value))
                {
                    value.Revisions.Add(cardRevision);
                }
                else
                {
                    data.Add(cardRevision.CardId, new CardFullDetails()
                    {
                        Id = cardRevision.CardId,
                        Revisions = [cardRevision]
                    });
                }
            }

            return data.Values;
        }

        public CardFullDetails? GetCard(int id)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            CardFullDetails output = new CardFullDetails()
            {
                Id = id
            };

            var cardRevisions = con.Query<CardRevision>("SELECT id, revisionNumber, cardId, name, typeId as cardType, ability, landscapeId as landscape, cost, attack, defense, setId as cardset FROM card_revision WHERE cardId = @CardId", new { CardId = id });

            if (cardRevisions == null || !cardRevisions.Any())
                return null;

            output.Revisions = cardRevisions.ToList();

            var revisionIds = cardRevisions.Select(x => x.Id).ToList();
            var cardImages = con.Query<CardImage>("SELECT id, revisionId, cardImageTypeId AS cardImageType, imageUrl FROM card_image WHERE revisionId IN @RevisionIds", new { RevisionIds = revisionIds });

            foreach (var rev in output.Revisions)
            {
                rev.CardImages = cardImages.Where(x => x.RevisionId == rev.Id).ToList();
            }

            return output;
        }
    }
}
