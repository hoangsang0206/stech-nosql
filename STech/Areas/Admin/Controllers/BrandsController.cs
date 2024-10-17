using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class BrandsController : Controller
    {
        private readonly IBrandService _brandService;

        public BrandsController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        public async Task<IActionResult> Index(string? sort_by, int page = 1)
        {
            if(page <= 0) 
            {
                page = 1;
            }

            (IEnumerable<Brand>, int) data = await _brandService.GetAll(sort_by, page);

            ViewBag.ActiveSidebar = "categories-brands";
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = data.Item2;

            return View(data.Item1);
        }
    }
}
