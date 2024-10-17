using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Filters;
using STech.Services;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin"), AdminAuthorize]
    public class ProductsController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;
        private readonly IBrandService _brandService;

        public ProductsController(ICategoryService categoryService, IProductService productService, IWarehouseService warehouseService, IBrandService brandService)
        {
            _categoryService = categoryService;
            _productService = productService;
            _warehouseService = warehouseService;
            _brandService = brandService;
        }

        public async Task<IActionResult> Index(string? brands, string? categories, string? status, string? price_range, 
            string? warehouse_id, string? sort, int page = 1, string view_type = "view-list")
        {
            var (products, totalPages) = await _productService.GetProducts(brands, categories, status, price_range, warehouse_id, sort, page);

            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();
            IEnumerable<Brand> _brands = await _brandService.GetAll(false);
            IEnumerable<Category> _categories = await _categoryService.GetAll(false);

            ViewBag.Warehouses = warehouses;
            ViewBag.Brands = _brands;
            ViewBag.Categories = _categories;

            ViewBag.ViewType = view_type;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;


            ViewBag.ActiveSidebar = "products";

            return View(products);
        }

        [Route("/admin/products/search/{query}")]
        public async Task<IActionResult> Search(string query, string? warehouse_id, string? sort, int page = 1, string view_type = "view-list")
        {
            var (products, totalPages) = await _productService.SearchProducts(query, page, sort, warehouse_id);


            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();
            IEnumerable<Brand> _brands = await _brandService.GetAll(false);
            IEnumerable<Category> _categories = await _categoryService.GetAll(false);

            ViewBag.Warehouses = warehouses;
            ViewBag.Brands = _brands;
            ViewBag.Categories = _categories;

            ViewBag.SearchValue = query;
            ViewBag.ViewType = view_type;
            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;


            ViewBag.ActiveSidebar = "products";

            return View("Index", products);
        }

        [Route("/admin/products/1/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            ProductMVM? product = await _productService.GetProduct(id);

            if (product == null)
            {
                return LocalRedirect("/admin/products");
            }

            IEnumerable<Category> categories = await _categoryService.GetAll(false);
            IEnumerable<Brand> brands = await _brandService.GetAll(false);

            ViewBag.ActiveSidebar = "products";
            return View(new Tuple<ProductMVM, IEnumerable<Category>, IEnumerable<Brand>>(product, categories, brands));
        }

        public async Task<IActionResult> Create()
        {
            IEnumerable<Warehouse> warehouses = await _warehouseService.GetWarehouses();
            IEnumerable<Brand> brands = await _brandService.GetAll(false);
            IEnumerable<Category> categories = await _categoryService.GetAll(false);

            ViewBag.ActiveSidebar = "products";

            return View(new Tuple<IEnumerable<Category>, IEnumerable<Brand>>(categories, brands));
        }
    }
}
