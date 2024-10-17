using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;
using STech.Utils;

namespace STech.Controllers
{
    [Route("/collections/{id}")]
    public class CollectionsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CollectionsController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string id, string? brands, string? price_range, string? sort, int page = 1)
        {
            if(id == null)
            {
                return NotFound();
            }

            int totalPage = 1;
            IEnumerable<Product> products = new List<Product>();
            Category? category;
            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();
            string title = "STech";

            if (id == "all")
            {
                (products, totalPage) = await _productService.GetProducts(brands, null, null, price_range, null, sort, page);
                breadcrumbs.Add(new Breadcrumb("Tất cả sản phẩm", ""));
                title = "Tất cả sản phẩm - STech";
            }
            else
            {
                category = await _categoryService.GetOne(id);
                if(category == null)
                {
                    return NotFound();
                }

                (products, totalPage) = await _productService.GetByCategory(id, page, sort);
                breadcrumbs.Add(new Breadcrumb("Danh sách sản phẩm", "/collections/all"));
                breadcrumbs.Add(new Breadcrumb(category.CategoryName, ""));
                title = "Danh sách " + category.CategoryName;
            }

            ViewBag.Sort = sort;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;

            ViewData["Title"] = title;
            return View(new Tuple<IEnumerable<Product>, List<Breadcrumb>>(products, breadcrumbs));
        }
    }
}
