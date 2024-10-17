using STech.Data.Models;

namespace STech.Data.MongoViewModels
{
    public class ReviewReplyMVM
    {
        public int Id { get; set; }

        public int ReviewId { get; set; }

        public string Content { get; set; } = null!;

        public DateTime ReplyDate { get; set; }

        public bool? IsRead { get; set; }

        public string UserReplyId { get; set; } = null!;

        public User? UserReply { get; set; }
    }
}
