using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;

        public ProductController(IProductService productService, IReviewService reviewService)
        {
            _productService = productService;
            _reviewService = reviewService;
        }

        [Route("product/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            if(id == null)
            {
                return NotFound();
            }

            ProductMVM? product = await _productService.GetProduct(id);
            if (product == null || product.IsDeleted == true)
            {
                return NotFound();
            }

            IEnumerable<Product> similarProducts = new List<Product>();
            List<Breadcrumb> breadcrumbs = new List<Breadcrumb>();

            Category? pCategory = product.Category;
            if(pCategory != null)
            {
                breadcrumbs.Add(new Breadcrumb(pCategory.CategoryName, $"/collections/{pCategory.CategoryId}"));
                similarProducts = await _productService.GetSimilarProducts(pCategory.CategoryId, 5);
            }
            breadcrumbs.Add(new Breadcrumb(product.ProductName, ""));

            return View(new Tuple<ProductMVM, IEnumerable<Product>, List<Breadcrumb>>(product, similarProducts, breadcrumbs));
        }
    }
}
