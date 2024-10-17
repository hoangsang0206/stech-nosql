using System.ComponentModel.DataAnnotations;

namespace STech.Data.ViewModels
{
    public class LoginVM
    {
        [Key]
        [Display(Name = "Tên đăng nhập")]
        [Required(ErrorMessage = "* Tên đăng nhập không để trống")]
        public string UserName { get; set; } = null!;

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "* Mật khẩu không để trống")]
        public string Password { get; set; } = null!;

        public string? ReturnUrl { get; set; }

        public string? CaptchaResponse { get; set; }
    }
}
