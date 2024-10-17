using Microsoft.AspNetCore.Mvc;
using STech.Filters;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), AdminAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {

            ViewBag.ActiveSidebar = "home";
            return View();
        }
    }
}
