using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using STech.Filters;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        [Route("admin/login")]
        public IActionResult Login()
        {
            return View();
        }

        [Route("admin/logout"), AdminAuthorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return LocalRedirect("/admin/login");
        }
    }
}
