using CornDome.Models;

namespace CornDome.Repository
{
    public interface IFeedbackRepository
    {
        Task<bool> AddFeedbackAsync(FeedbackRequest feedbackRequest);
        Task<IEnumerable<FeedbackRequest>> GetAllFeedback();
        Task<bool> DeleteFeedback(int id);
    }
    public class FeedbackRepository(MainContext context) : IFeedbackRepository
    {
        public async Task<bool> AddFeedbackAsync(FeedbackRequest feedbackRequest)
        {
            try
            {
                context.Add(feedbackRequest);

                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<FeedbackRequest>> GetAllFeedback()
        {
            return context.CardFeedbacks;
        }

        public async Task<bool> DeleteFeedback(int id)
        {
            try
            {
                var feedbackRequest = context.CardFeedbacks.FirstOrDefault(x => x.Id == id);
                if (feedbackRequest != null)
                {
                    context.Remove(feedbackRequest);
                    await context.SaveChangesAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
