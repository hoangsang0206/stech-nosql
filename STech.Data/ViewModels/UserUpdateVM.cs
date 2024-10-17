using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using STech.Data.Validations;

namespace STech.Data.ViewModels
{
    public class UserUpdateVM
    {
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "* Vui lòng nhập Họ tên")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Email")]
        [Required(ErrorMessage = "* Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "* Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Display(Name = "SĐT")]
        [Required(ErrorMessage = "* Vui lòng nhập SĐT")]
        [RegularExpression(@"^(0[1-9]\d{8,9})$|^(84[1-9]\d{8,9})$|^\+84[1-9]\d{8,9}$", ErrorMessage = "* Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; } = null!;

        [DateOfBirth(ErrorMessage = "* Ngày sinh không hợp lệ")]
        public DateOnly? DOB { get; set; }

        [Gender(ErrorMessage = "Giới tính không hợp lệ")]
        public string? Gender { get; set; } = null!;
    }
}
