using MongoDB.Bson;
using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services.Utils;

namespace STech.Services.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IMongoCollection<Product> _collection;
        private readonly IUserService _userService;

        public ReviewService(StechDbContext context, IUserService userService)
        {
            _collection = context.GetCollection<Product>("Products");
            _userService = userService;
        }
 
        public async Task<IEnumerable<ReviewMVM>> GetReviews(string productId, string? sort_by)
        {
            Product? product = await _collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

            return product.Reviews.SelectReview(_userService);
        }

        public async Task<(IEnumerable<ReviewMVM>, ReviewOverview, int, int, int)> GetReviews(string productId, int reviewsPerPage, int numOfReplies, 
            string? sort_by, string? filter_by, string? current_user, int page = 1)
        {
            Product? product = await _collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

            IEnumerable<ReviewMVM> reviews = product.Reviews.SelectReview(_userService, numOfReplies);

            int totalReviews = reviews.Count();
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filter_by).Sort(sort_by);
            int filteredCount = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)filteredCount / reviewsPerPage);

            int remainingReviews = filteredCount - reviewsPerPage * (page > totalPages ? totalPages : page);
                
            return (
                reviews.Paginate(page, reviewsPerPage), 
                overview,
                totalPages,
                totalReviews,
                remainingReviews > 0 ? remainingReviews : 0
            );
        }

        public async Task<(IEnumerable<ReviewMVM>, int)> GetReviewsWithProduct(int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            IEnumerable<Product> products = await _collection.Find(p => p.Reviews.Count() > 0).ToListAsync();
            List<ReviewMVM> reviews = new List<ReviewMVM>();

            foreach(Product p in products)
            {
                reviews.AddRange(p.Reviews.SelectReviewWithProduct(p.ProductId, _userService, _collection));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if(status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true).ToList();
                } 
                else if(status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true).ToList();
                }
            }

            reviews = reviews.Filter(filter_by).Sort(sort_by).ToList();

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews.Paginate(page, reviewsPerPage),
                totalPages
            );
        }

        public async Task<(IEnumerable<ReviewMVM>, int)> GetProductReviews(string productId, int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            Product product = await _collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();
            IEnumerable<ReviewMVM> reviews = product.Reviews.SelectReviewWithProduct(product.ProductId, _userService, _collection);

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true).ToList();
                }
                else if (status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true).ToList();
                }
            }

            reviews = reviews.Filter(filter_by).Sort(sort_by).ToList();

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews.Paginate(page, reviewsPerPage),
                totalPages
            );
        }

        public async Task<(IEnumerable<ReviewMVM>, int)> SearchReviewsWithProduct(string query, int reviewsPerPage, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            string[] keywords = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            FilterDefinition<Product> filter = Builders<Product>.Filter.Or(
                Builders<Product>.Filter.Eq(p => p.ProductId, query),
                Builders<Product>.Filter.And(keywords.Select(key =>
                    Builders<Product>.Filter.Regex(p => p.ProductName, new BsonRegularExpression(key, "i")))
                )
            );

            IEnumerable<Product> products = await _collection
                .Find(filter)
                .ToListAsync();

            List<ReviewMVM> reviews = new List<ReviewMVM>();

            foreach (Product p in products)
            {
                reviews.AddRange(p.Reviews.SelectReviewWithProduct(p.ProductId, _userService, _collection));
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded == true).ToList();
                }
                else if (status == "not-approved")
                {
                    reviews = reviews.Where(r => r.IsProceeded != true).ToList();
                }
            }

            reviews = reviews.Filter(filter_by).Sort(sort_by).ToList();

            int totalReviews = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)totalReviews / reviewsPerPage);

            return (
                reviews.Paginate(page, reviewsPerPage),
                totalPages
            );
        }

        public async Task<(IEnumerable<ReviewMVM>, ReviewOverview, int, int, int)> GetApprovedReviews(string productId, int reviewsPerPage, int numOfReplies, string? sort_by, string? filter_by, string? current_user, int page = 1)
        {
            Product? product = await _collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

            IEnumerable<ReviewMVM> reviews = product.Reviews
                .Where(r => r.IsProceeded == true)
                .SelectReview(_userService, numOfReplies, current_user);

            int totalReviews = reviews.Count();
            ReviewOverview overview = reviews.GetReviewOverview();

            reviews = reviews.Filter(filter_by).Sort(sort_by);
            int filteredCount = reviews.Count();
            int totalPages = (int)Math.Ceiling((double)filteredCount / reviewsPerPage);

            int remainingReviews = filteredCount - reviewsPerPage * (page > totalPages ? totalPages : page);

            return (
                reviews.Paginate(page, reviewsPerPage).Filter(filter_by).Sort(sort_by),
                overview,
                totalPages,
                totalReviews,
                remainingReviews > 0 ? remainingReviews : 0
            );
        }

        

        public async Task<ReviewMVM?> GetReview(int reviewId)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter
                .ElemMatch(p => p.Reviews, Builders<Review>.Filter.Eq(r => r.RId, reviewId));

            Product? product = await _collection.Find(filter).FirstOrDefaultAsync();

            ReviewMVM? review = product.Reviews.SelectReviewDetail(_userService)
                .Where(r => r.Id == reviewId).FirstOrDefault();

            if(review != null) review.ProductId = product.ProductId;

            return review;
        }

        public async Task<(IEnumerable<ReviewReplyMVM>, int, int, int)> GetReviewReplies(int reviewId, int page, int repliesPerPage)
        {
            ReviewMVM review = await GetReview(reviewId) ?? new ReviewMVM();

            IEnumerable<ReviewReplyMVM> replies = review.ReviewReplies
                .Where(rp => rp.ReviewId == reviewId)
                .Select(rp => new ReviewReplyMVM
                {
                    Id = rp.Id,
                    ReviewId = rp.ReviewId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    UserReply = rp.UserReply,
                })
                .OrderBy(rp => rp.ReplyDate)
                .ToList();

            int totalReplies = replies.Count();
            int totalPages = (int)Math.Ceiling((double)totalReplies / repliesPerPage);
            int remainingReplies = totalReplies - (page > totalPages ? totalPages : page) * repliesPerPage;

            return (
                    replies.Paginate(page, repliesPerPage), 
                    totalPages, 
                    totalReplies, 
                    remainingReplies > 0 ? remainingReplies : 0
                );
        }

        public async Task<IEnumerable<ReviewReplyMVM>> GetReviewReplies(int reviewId)
        {

            ReviewMVM review = await GetReview(reviewId) ?? new ReviewMVM();

            return review.ReviewReplies
                .Where(rp => rp.ReviewId == reviewId)
                .Select(rp => new ReviewReplyMVM
                {
                    Id = rp.Id,
                    ReviewId = rp.ReviewId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    UserReply = rp.UserReplyId == null ? null : _userService.GetUserById(rp.UserReplyId).Result,
                })
                .OrderBy(rp => rp.ReplyDate)
                .ToList();
        }

        public async Task<bool> CreateReview(string productId, Review review)
        {
            review.RId = RandomUtils.RandomNumbers(1, 999999999);

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Push(u => u.Reviews, review);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == productId, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteReview(string productId, int reviewId)
        {
            UpdateDefinition<Product> update = Builders<Product>.Update
                .PullFilter(u => u.Reviews, r => r.RId == reviewId);

            UpdateResult result = await _collection.UpdateOneAsync(p => p.ProductId == productId, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> CreateReviewReply(string productId, int reviewId, ReviewReply reviewReply)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.ProductId, productId),
                Builders<Product>.Filter.ElemMatch(p => p.Reviews, r => r.RId == reviewId)
            );

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Push("Reviews.$.ReviewReplies", reviewReply);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> LikeReview(string productId, int reviewId, string userId)
        {
            Product? product = await _collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync();

            Review? review = product.Reviews.Where(r => r.RId == reviewId).FirstOrDefault();
            if (review == null)
            {
                return false;
            }

            ReviewLike? reviewLike = review.ReviewLikes.Where(rl => rl.UserId == userId).FirstOrDefault();
            if (reviewLike != null)
            {
                return false;
            } 

            ReviewLike newReviewLike = new ReviewLike
            {
                UserId = userId,
                LikeDate = DateTime.Now,
            };

            FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.ProductId, product.ProductId),
                Builders<Product>.Filter.ElemMatch(p => p.Reviews, r => r.RId == reviewId)
            );

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Push("Reviews.$.ReviewLikes", newReviewLike)
                .Inc("Reviews.$.TotalLike", +1);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> UnLikeReview(string productId, int reviewId, string userId)
        {
            return false;
        }

        public async Task<bool> ApproveReview(string productId, int reviewId)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(p => p.ProductId, productId),
                Builders<Product>.Filter.ElemMatch(p => p.Reviews, r => r.RId == reviewId)
            );

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set("Reviews.$.IsProceeded", true);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<bool> MarkAllRepliesAsRead(string productId, int reviewId)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.And(
                 Builders<Product>.Filter.Eq(p => p.ProductId, productId),
                 Builders<Product>.Filter.ElemMatch(p => p.Reviews, r => r.RId == reviewId)
             );

            UpdateDefinition<Product> update = Builders<Product>.Update
                .Set("Reviews.$.ReviewReplies.$[].IsRead", true);

            UpdateResult result = await _collection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}
