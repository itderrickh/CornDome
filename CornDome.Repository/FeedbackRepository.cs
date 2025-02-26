using CornDome.Models;
using Dapper;
using System.Data.SQLite;

namespace CornDome.Repository
{
    public interface IFeedbackRepository
    {
        int AddFeedback(FeedbackRequest feedbackRequest);
        List<FeedbackRequest> GetAllFeedback();
        int DeleteFeedback(int id);
    }
    public class FeedbackRepository(UserRepositoryConfig config) : IFeedbackRepository
    {
        private const string FEEDBACK_INSERT = @"
            INSERT INTO CardFeedback (Feedback, CardId, RevisionId) VALUES (@Feedback, @CardId, @RevisionId);
        ";
        public int AddFeedback(FeedbackRequest feedbackRequest)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var inserted = con.Execute(FEEDBACK_INSERT, new { feedbackRequest.Feedback, feedbackRequest.CardId, feedbackRequest.RevisionId });

            return inserted;
        }

        public List<FeedbackRequest> GetAllFeedback()
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var feedback = con.Query<FeedbackRequest>("SELECT Feedback, CardId, RevisionId, Id FROM CardFeedback;");

            return feedback.ToList();
        }

        public int DeleteFeedback(int id)
        {
            using var con = new SQLiteConnection(config.DbPath);
            con.Open();

            var deleted = con.Execute("DELETE FROM CardFeedback WHERE Id = @Id", new { Id = id });
            return deleted;
        }
    }
}
