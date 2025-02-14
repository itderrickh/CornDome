using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public class SqliteCardRepository(SqliteRepositoryConfig config) : ICardRepository
    {
        private static readonly string SELECT_QUERY = @"
            SELECT cr.cardId AS id, cr.landscapeId AS landscape, cr.typeId AS type, cr.attack, cr.defense, cr.cost, cr.ability, cr.setId AS [set], cr.revisionNumber AS revision, ci.imageUrl
            FROM card_revision AS cr
            LEFT JOIN card_image AS ci ON ci.revisionId = cr.id
            WHERE cr.id IN (
	            SELECT id FROM
	             (
		            SELECT cardId, id, MIN(revisionNumber) FROM card_revision
		            GROUP BY cardId
	            )
            )
            ORDER BY cr.cardId";

        public IEnumerable<Card> GetAll()
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var res = con.Query<Card>(SELECT_QUERY);

            return res;
        }
    }
}
