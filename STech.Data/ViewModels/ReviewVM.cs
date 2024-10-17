using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class ReviewVM
    {
        [Required(ErrorMessage = "Vui lòng nhập nội dung đánh giá")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn số sao")]
        [Range(1, 5, ErrorMessage = "Số sao phải từ 1 đến 5")]
        public int Rating { get; set; }

        [Required]
        public string ProductId { get; set; } = null!;

        public Reviewer? ReviewerInfo { get; set; }

        public class Reviewer
        {
            [Required(ErrorMessage = "Vui lòng nhập tên hiển thị")]
            public string ReviewerName { get; set; } = null!;

            [Required(ErrorMessage = "Vui lòng nhập email")]
            public string ReviewerEmail { get; set; } = null!;

            public string? ReviewerPhone { get; set; } = null!;
        }
    }
}
