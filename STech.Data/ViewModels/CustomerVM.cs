

using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class CustomerVM
    {
        [Required(ErrorMessage = "* Tên khách hàng không để trống")]
        public string CustomerName { get; set; } = null!;

        [Required(ErrorMessage = "* Số điện thoại không để trống")]
        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
        public string Phone { get; set; } = null!;

        [EmailAddress(ErrorMessage = "* Email không hợp lệ")]
        public string? Email { get; set; }

        [Required]
        public string CityCode { get; set; } = null!;

        [Required]
        public string DistrictCode { get; set; } = null!;

        [Required]
        public string WardCode { get; set; } = null!;

        [Required]
        public string Address { get; set; } = null!;

        public DateOnly? Dob { get; set; }

        [Required(ErrorMessage = "* Chọn giới tính")]
        public string Gender { get; set; } = null!;
    }
}
