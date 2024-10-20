using MongoDB.Driver;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;

namespace STech.Services.Utils
{
    public static class ReviewUtils
    {
        public static IEnumerable<ReviewMVM> SelectReview(this IEnumerable<Review> reviews, IUserService userService)
        {
            return reviews.Select(r => new ReviewMVM
            {
                Id = r.RId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                User = r.UserId == null ? null : userService.GetUserById(r.UserId).Result,
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReplyMVM
                {
                    Id = rp.RRId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = rp.UserReplyId == null ? null : userService.GetUserById(rp.UserReplyId).Result,
                }).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IEnumerable<ReviewMVM> SelectReviewDetail(this IEnumerable<Review> reviews, IUserService userService)
        {
            return reviews.Select(r => new ReviewMVM
            {
                Id = r.RId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                ReviewerEmail = r.ReviewerEmail,
                ReviewerPhone = r.ReviewerPhone,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                User = r.UserId == null ? null : userService.GetUserById(r.UserId).Result,
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReplyMVM
                {
                    Id = rp.RRId,
                    ReviewId = r.RId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = rp.UserReplyId == null ? null : userService.GetUserById(rp.UserReplyId).Result,
                }).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IEnumerable<ReviewMVM> SelectReview(this IEnumerable<Review> reviews, 
            IUserService userService, int numOfReplies)
        {
            return reviews.Select(r => new ReviewMVM
            {
                Id = r.RId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                User = r.UserId == null ? null : userService.GetUserById(r.UserId).Result,
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReplyMVM
                {
                    Id = rp.RRId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = rp.UserReplyId == null ? null : userService.GetUserById(rp.UserReplyId).Result,
                }).Take(numOfReplies).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IEnumerable<ReviewMVM> SelectReview(this IEnumerable<Review> reviews, 
            IUserService userService,
            int numOfReplies, string? userId)
        {
            return reviews.Select(r => new ReviewMVM
            {
                Id = r.RId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                IsLiked = r.ReviewLikes.Any(rl => rl.UserId == userId),
                User = r.UserId == null ? null : userService.GetUserById(r.UserId).Result,
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReplyMVM
                {
                    Id = rp.RRId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = rp.UserReplyId == null ? null : userService.GetUserById(rp.UserReplyId).Result,
                }).Take(numOfReplies).OrderBy(rp => rp.ReplyDate).ToList(),
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static IEnumerable<ReviewMVM> SelectReviewWithProduct(this IEnumerable<Review> reviews, 
            string productId,
            IUserService userService,
            IMongoCollection<Product> collection)
        {
            return reviews.Select(r => new ReviewMVM
            {
                Id = r.RId,
                ProductId = productId,
                Rating = r.Rating,
                Content = r.Content,
                CreateAt = r.CreateAt,
                ReviewerName = r.ReviewerName,
                TotalLike = r.TotalLike,
                IsPurchased = r.IsPurchased,
                IsProceeded = r.IsProceeded,
                User = r.UserId == null ? null : userService.GetUserById(r.UserId).Result,
                ReviewImages = r.ReviewImages,
                ReviewReplies = r.ReviewReplies.Select(rp => new ReviewReplyMVM
                {
                    Id = rp.RRId,
                    Content = rp.Content,
                    ReplyDate = rp.ReplyDate,
                    IsRead = rp.IsRead,
                    UserReply = rp.UserReplyId == null ? null : userService.GetUserById(rp.UserReplyId).Result,
                }).OrderBy(rp => rp.ReplyDate).ToList(),
                Product = collection.Find(p => p.ProductId == productId).FirstOrDefaultAsync().Result,
            })
            .OrderByDescending(r => r.CreateAt);
        }

        public static ReviewOverview GetReviewOverview(this IEnumerable<ReviewMVM> reviews)
        {
            int totalReviews = reviews.Count();
            double averageRating = totalReviews > 0 ? Math.Round(reviews.Average(r => r.Rating), 1) : 0;
            int total5Star = reviews.Count(r => r.Rating == 5);
            int total4Star = reviews.Count(r => r.Rating == 4);
            int total3Star = reviews.Count(r => r.Rating == 3);
            int total2Star = reviews.Count(r => r.Rating == 2);
            int total1Star = reviews.Count(r => r.Rating == 1);

            return new ReviewOverview
            {
                AverageRating = averageRating,
                Total5StarReviews = total5Star,
                Total4StarReviews = total4Star,
                Total3StarReviews = total3Star,
                Total2StarReviews = total2Star,
                Total1StarReviews = total1Star,
            };
        }


        public static IEnumerable<ReviewMVM> Paginate(this IEnumerable<ReviewMVM> reviews, int page, int reviewsPerpage)
        {
            if(page <= 0)
            {
                page = 1;
            }

            return reviews.Skip((page - 1) * reviewsPerpage).Take(reviewsPerpage);
        }

        public static IEnumerable<ReviewReplyMVM> Paginate(this IEnumerable<ReviewReplyMVM> replies, int page, int repliesPerPage)
        {
            if(page <= 0)
            {
                page = 1;
            }

            return replies.Skip((page - 1) * repliesPerPage).Take(repliesPerPage);
        }

        public static IEnumerable<ReviewMVM> Sort(this IEnumerable<ReviewMVM> reviews, string? sort_by)
        {
            switch (sort_by)
            {
                case "newest":
                    return reviews.OrderByDescending(r => r.CreateAt);
                case "oldest":
                    return reviews.OrderBy(r => r.CreateAt);
                case "rating-ascending":
                    return reviews.OrderBy(r => r.Rating);
                case "rating-descending":
                    return reviews.OrderByDescending(r => r.Rating);
                case "most-liked":
                    return reviews.OrderByDescending(r => r.TotalLike);
                default:
                    return reviews.OrderByDescending(r => r.CreateAt);
            }
        }

        public static IEnumerable<ReviewMVM> Filter(this IEnumerable<ReviewMVM> reviews, string? filter_by)
        {
            switch (filter_by)
            {
                case "purchased":
                    return reviews.Where(r => r.IsPurchased == true);
                case "not-purchased":
                    return reviews.Where(r => r.IsPurchased != true);
                case "with-images":
                    return reviews.Where(r => r.ReviewImages.Count > 0);
                case "5-stars":
                    return reviews.Where(r => r.Rating == 5);
                case "4-stars":
                    return reviews.Where(r => r.Rating == 4);
                case "3-stars":
                    return reviews.Where(r => r.Rating == 3);
                case "2-stars":
                    return reviews.Where(r => r.Rating == 2);
                case "1-star":
                    return reviews.Where(r => r.Rating == 1);
                default:
                    return reviews;
            }
        }
    }
}
