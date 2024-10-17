using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class ChangePasswordVM
    {
        [Display(Name = "Mật khẩu cũ")]
        [Required(ErrorMessage = "* Mật khẩu cũ không để trống")]
        [MaxLength(30, ErrorMessage = "* Mật khẩu tối đa 30 kí tự")]
        public string OldPassword { get; set; } = null!;

        [Display(Name = "Mật khẩu mới")]
        [Required(ErrorMessage = "* Mật khẩu mới không để trống")]
        public string NewPassword { get; set; } = null!;

        [Display(Name = "Xác nhận mật khẩu")]
        [Required(ErrorMessage = "* Vui lòng xác nhận mật khẩu")]
        [Compare("NewPassword", ErrorMessage = "* Xác nhập mật khảu không đúng")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
