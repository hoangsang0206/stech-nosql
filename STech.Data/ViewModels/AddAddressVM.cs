using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class AddAddressVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "* Địa chỉ không để trống")]
        public string Address { get; set; } = null!;

        [Required(ErrorMessage = "* Phường/xã không để trống")]
        public string WardCode { get; set; } = null!;

        [Required(ErrorMessage = "* Quận/huyện không để trống")]
        public string DistrictCode { get; set; } = null!;

        [Required(ErrorMessage = "* Tỉnh/thành phố không để trống")]
        public string CityCode { get; set; } = null!;

        [Required(ErrorMessage = "* Loại địa chỉ không để trống")]
        public string Type { get; set; } = "home";

        [Required(ErrorMessage = "* Tên người nhận không để trống")]
        public string RecipientName { get; set; } = null!;

        [Required(ErrorMessage = "* Số điện thoại không để trống")]
        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
        public string RecipientPhone { get; set; } = null!;
    }
}
