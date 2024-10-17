using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Services;

namespace STech.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Search(string q)
        {
            var (products, totalPage) = await _productService.SearchByName(q, 1, null);
            return Ok(new ApiResponse
            {
                Status = true,
                Data = products
            });

        }
    }
}
