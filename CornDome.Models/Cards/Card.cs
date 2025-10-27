using System.ComponentModel.DataAnnotations.Schema;

namespace CornDome.Models.Cards
{
    [Table("card")]
    public class Card
    {
        public int Id { get; set; }
        public bool IsCustomCard { get; set; }
        public IList<CardRevision> Revisions { get; set; } = [];
        public CardRevision LatestRevision
        {
            get
            {
                return Revisions
                    .OrderByDescending(x => x.RevisionNumber)
                    .FirstOrDefault();
            }
        }
    }
}
