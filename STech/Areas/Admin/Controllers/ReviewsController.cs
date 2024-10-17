using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Filters;
using STech.Services;

namespace STech.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthorize]
    public class ReviewsController : Controller
    {
        private readonly IUserService _userService;
        private readonly IReviewService _reviewService;
        private readonly IProductService _productService;

        public ReviewsController(IUserService userService, IReviewService reviewService, IProductService productService)
        {
            _userService = userService;
            _reviewService = reviewService;
            _productService = productService;
        }

        public async Task<IActionResult> Index(string? search, string? sort_by, string? status, string? filter_by, int page = 1)
        {
            if(page <= 1)
            {
                page = 1;          
            }

            var (reviews, totalPages) = search != null 
                ? await _reviewService.SearchReviewsWithProduct(search, 40, sort_by, status, filter_by, page)
                : await _reviewService.GetReviewsWithProduct( 40, sort_by, status, filter_by, page);

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;

            ViewBag.ActiveSidebar = "reviews";
            return View(reviews);
        }

        [Route("/admin/reviews/1/{rId}")]
        public async Task<IActionResult> Detail(int rId)
        {
            ReviewMVM? review = await _reviewService.GetReview(rId);
           
            if(review == null)
            {
                return LocalRedirect("/admin/reviews");
            }

            ProductMVM? product = await _productService.GetProduct(review.ProductId);

            ViewBag.ActiveSidebar = "reviews";
            return View(new Tuple<ReviewMVM, ProductMVM?>(review, product));
        }

        [Route("/admin/reviews/product/{pId}")]
        public async Task<IActionResult> ProductReviews(string pId)
        {
            ProductMVM? product = await _productService.GetProductWithBasicInfo(pId);
            if(product == null)
            {
                return LocalRedirect($"/admin/reviews?search={pId}");
            }

            var (reviews, overview, totalPages, totalReviews, remainingReviews) = 
                await _reviewService.GetReviews(pId, 40, 0, null, null, null);

            return View(new Tuple<ProductMVM, IEnumerable<ReviewMVM>>(product, reviews));
        }
    }
}
