using CornDome.Models;
using Dapper;

namespace CornDome.Repository
{
    public interface IFeedbackRepository
    {
        int AddFeedback(FeedbackRequest feedbackRequest);
        List<FeedbackRequest> GetAllFeedback();
        int DeleteFeedback(int id);
    }
    public class FeedbackRepository(IDbConnectionFactory dbConnectionFactory) : IFeedbackRepository
    {
        private readonly IDbConnectionFactory dbConnectionFactory = dbConnectionFactory;
        private const string FEEDBACK_INSERT = @"
            INSERT INTO CardFeedback (Feedback, CardId, RevisionId) VALUES (@Feedback, @CardId, @RevisionId);
        ";
        public int AddFeedback(FeedbackRequest feedbackRequest)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var inserted = con.Execute(FEEDBACK_INSERT, new { feedbackRequest.Feedback, feedbackRequest.CardId, feedbackRequest.RevisionId });

            return inserted;
        }

        public List<FeedbackRequest> GetAllFeedback()
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var feedback = con.Query<FeedbackRequest>("SELECT Feedback, CardId, RevisionId, Id FROM CardFeedback;");

            return feedback.ToList();
        }

        public int DeleteFeedback(int id)
        {
            using var con = dbConnectionFactory.CreateMasterDbConnection();

            var deleted = con.Execute("DELETE FROM CardFeedback WHERE Id = @Id", new { Id = id });
            return deleted;
        }
    }
}
