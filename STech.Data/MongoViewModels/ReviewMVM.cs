using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class ReviewMVM
    {
        public int Id { get; set; }

        public string Content { get; set; } = null!;

        public int Rating { get; set; }

        public DateTime CreateAt { get; set; }

        public int TotalLike { get; set; }

        public string? ReviewerName { get; set; }

        public string? ReviewerEmail { get; set; }

        public string? ReviewerPhone { get; set; }

        public string? UserId { get; set; }

        public string ProductId { get; set; } = null!;

        public bool? IsPurchased { get; set; }

        public bool? IsProceeded { get; set; }

        public bool? IsLiked { get; set; }

        public Product? Product { get; set; }

        public User? User { get; set; }

        public List<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();

        public List<ReviewReplyMVM> ReviewReplies { get; set; } = new List<ReviewReplyMVM>();

        public List<ReviewLike> ReviewLikes { get; set; } = new List<ReviewLike>();
    }
}
