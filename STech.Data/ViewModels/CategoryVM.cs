using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class CategoryVM
    {
        [Required(ErrorMessage = "Mã danh mục không để trống")]
        [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã danh mục không chứa kí tự đặc biệt (ngoại trừ -, _) và khoảng trống")]
        public string CategoryId { get; set; } = null!;

        [Required(ErrorMessage = "Tên danh mục không để trống")]
        public string CategoryName { get; set; } = null!;

        public string? ImageSrc { get; set; }
    }
}
