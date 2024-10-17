using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class BrandVM
    {
        [Required(ErrorMessage = "Mã hãng không để trống")]
        [RegularExpression(@"^[a-zA-Z0-9-_]*$", ErrorMessage = "Mã hãng không chứa kí tự đặc biệt (ngoại trừ -, _) và khoảng trống")]
        public string BrandId { get; set; } = null!;

        [Required(ErrorMessage = "Tên hãng không để trống")]
        public string BrandName { get; set; } = null!;

        public string? Address { get; set; }

        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
        public string? Phone { get; set; }

        public string? LogoSrc { get; set; }
    }
}
