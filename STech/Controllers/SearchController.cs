using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;

namespace STech.Controllers
{
    public class SearchController : Controller
    {
        private readonly IProductService _productService;

        public SearchController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index(string q, string? sort, int page = 1)
        {
            var (products, totalPage) = await _productService.SearchByName(q, page, sort);

            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>
            {
                new Breadcrumb("Tìm kiếm", ""),
                new Breadcrumb(q, "")
            };

            ViewBag.Search = q;
            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;

            return View(new Tuple<IEnumerable<Product>, List<Breadcrumb>>(products, breadcrumbs));
        }
    }
}
