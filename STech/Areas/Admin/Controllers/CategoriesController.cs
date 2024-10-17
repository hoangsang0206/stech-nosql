using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? sort_by, int page = 1)
        {
            if(page <= 0)
            {
                page = 1;
            }

            (IEnumerable<CategoryMVM>, int) data = await _categoryService.GetAllWithProducts(sort_by, page);

            ViewBag.ActiveSidebar = "categories-brands";
            ViewBag.SortBy = sort_by;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = data.Item2;

            return View(data.Item1);
        }
    }
}
