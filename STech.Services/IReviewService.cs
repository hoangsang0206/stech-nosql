using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;

namespace STech.Services
{
    public interface IReviewService
    {
        Task<(IEnumerable<ReviewMVM>, ReviewOverview, int, int, int)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, string? sort_by, string? filter_by, string? current_user, int page = 1);
        Task<(IEnumerable<ReviewMVM>, ReviewOverview, int, int, int)> GetApprovedReviews(string productId, int reviewsPerPage, int numOfReplies, string? sort_by, string? filter_by, string? current_user, int page = 1);
        Task<IEnumerable<ReviewMVM>> GetReviews(string productId, string? sort_by);
        Task<(IEnumerable<ReviewMVM>, int)> GetReviewsWithProduct(int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1);
        Task<(IEnumerable<ReviewMVM>, int)> SearchReviewsWithProduct(string query, int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1);
        Task<ReviewMVM?> GetReview(int reviewId);
        Task<(IEnumerable<ReviewReplyMVM>, int, int, int)> GetReviewReplies(int reviewId, int page, int repliesPerPage);
        Task<IEnumerable<ReviewReplyMVM>> GetReviewReplies(int reviewId);

        Task<bool> CreateReview(string productId, Review review);
        Task<bool> DeleteReview(string productId, int reviewId);

        Task<bool> CreateReviewReply(string productId, int reviewId, ReviewReply reviewReply);

        Task<bool> LikeReview(string productId, int reviewId, string userId);
        Task<bool> UnLikeReview(string productId, int reviewId, string userId);

        Task<bool> ApproveReview(string productId, int reviewId);

        Task<bool> MarkAllRepliesAsRead(string productId, int reviewId);
    }
}
