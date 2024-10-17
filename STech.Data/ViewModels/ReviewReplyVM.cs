using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class ReviewReplyVM
    {
        [Required]
        public string ProductId { get; set; } = null!;

        [Required]
        public int ReviewId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung")]
        public string Content { get; set; } = null!;
    }
}
