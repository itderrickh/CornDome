using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public class SqliteCardRepository(SqliteRepositoryConfig config) : ICardRepository
    {
        private Dictionary<int, CardFullDetails> FillOutRevisionDetails(IEnumerable<CardRevision> cardRevisions, IEnumerable<CardImage> cardImages)
        {
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

            return data;
        }

        public IEnumerable<CardFullDetails> GetAll()
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var cardRevisions = con.Query<CardRevision>("SELECT id, revisionNumber, cardId, name, typeId as cardType, ability, landscapeId as landscape, cost, attack, defense, setId as cardset FROM card_revision");

            var revisionIds = cardRevisions.Select(x => x.Id).ToList();
            var cardImages = con.Query<CardImage>("SELECT id, revisionId, cardImageTypeId AS cardImageType, imageUrl FROM card_image WHERE revisionId IN @RevisionIds", new { RevisionIds = revisionIds });

            var data = FillOutRevisionDetails(cardRevisions, cardImages);

            return data.Values
                .OrderBy(x => x.LatestRevision.Name);
        }

        public CardFullDetails? GetCard(int id)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            CardFullDetails output = new()
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

        public IEnumerable<CardFullDetails> GetCardsFromQuery(List<string> query)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var querystring = @"
                SELECT cr.id, cr.revisionNumber, cr.cardId, cr.name, cr.typeId as cardType, cr.ability, cr.landscapeId as landscape, cr.cost, cr.attack, cr.defense, cr.setId as cardset FROM (
	                SELECT inner_cr.cardId, MAX(inner_cr.revisionNumber) AS revId FROM card_revision inner_cr
	                LEFT JOIN card cd ON cd.Id = inner_cr.CardId
	                WHERE LOWER(inner_cr.name) IN @QueryList
	                GROUP BY inner_cr.CardId
                ) AS latestRevision
                LEFT JOIN card_revision AS cr ON latestRevision.cardId = cr.cardId AND latestRevision.revId = cr.revisionNumber
            ";

            var cardRevisions = con.Query<CardRevision>(querystring, new { QueryList = query });
            var revisionIds = cardRevisions.Select(x => x.Id).ToList();
            var cardImages = con.Query<CardImage>("SELECT id, revisionId, cardImageTypeId AS cardImageType, imageUrl FROM card_image WHERE revisionId IN @RevisionIds", new { RevisionIds = revisionIds });

            var data = FillOutRevisionDetails(cardRevisions, cardImages);

            return data.Values
                .OrderBy(x => x.LatestRevision.Name);
        }
    }
}
