using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STech.Data.ViewModels
{
    public class RegisterVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "* Tên đăng nhập không để trống")]
        [MaxLength(20, ErrorMessage = "* Tên đăng nhập tối đa 20 kí tự")]
        public string RegUserName { get; set; } = null!;
        
        [Display(Name = "Email")]
        [Required(ErrorMessage = "* Email không để trống")]
        [EmailAddress(ErrorMessage = "* Email không đúng định dạng")]
        public string Email { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "* Mật khẩu không để trống")]
        [MaxLength(30, ErrorMessage = "* Mật khẩu tối đa 30 kí tự")]
        public string RegPassword { get; set; } = null!;

        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "* Vui lòng xác nhận mật khẩu")]
        [Compare("RegPassword", ErrorMessage = "* Xác nhập mật khảu không đúng")]
        public string ConfirmPassword { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public string? CaptchaResponse { get; set; }
    }
}
