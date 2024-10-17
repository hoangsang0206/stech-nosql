using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using STech.Utils;

namespace STech.Areas.Admin.ApiControllers
{
    [AdminAuthorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly string IMAGE_PATH = "categories/";

        private readonly IBrandService _brandService;
        private readonly IAzureService _azureService;

        public BrandsController(IBrandService brandService, IAzureService azureService)
        {
            _brandService = brandService;
            _azureService = azureService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBrands(string? sort_by, int page = 1)
        {
            if (page <= 0)
            {
                page = 1;
            }

            (IEnumerable<Brand>, int) data = await _brandService.GetAll(sort_by, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    brands = data.Item1,
                    currentPage = page,
                    totalPages = data.Item2
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrand(string id)
        {
            Brand? brand = await _brandService.GetById(id);

            return Ok(new ApiResponse
            {
                Status = brand != null,
                StatusCode = brand == null ? 404 : 200,
                Message = brand == null ? "Không tìm thấy hãng này" : "",
                Data = brand,
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateBrand([FromForm] BrandVM brand, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string imageExtension = Path.GetExtension(image.FileName).ToLower();
                    if (!ImageUtils.CheckImageExtension(imageExtension))
                    {
                        return Ok(new ApiResponse
                        {
                            Status = false,
                            Message = "Hình ảnh hãng không hợp lệ"
                        });
                    }

                    byte[] imageBytes = ConvertFile.ConvertIFormFileToByteArray(image);
                    string path = $"{IMAGE_PATH}{brand.BrandId}{imageExtension}";

                    brand.LogoSrc = await _azureService.UploadImage(path, imageBytes);
                }

                bool result = await _brandService.Create(new Brand
                {
                    BrandId = brand.BrandId,
                    BrandName = brand.BrandName,
                    LogoSrc = brand.LogoSrc,
                    Address = brand.Address,
                    Phone = brand.Phone,
                });

                return Ok(new ApiResponse
                {
                    Status = result,
                    Message = result ? "Thêm mới hãng thành công" : "Theemm mới hãng thất bại"
                });
            }

            return BadRequest();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBrand([FromForm] BrandVM brand, IFormFile? image)
        {
            if (ModelState.IsValid)
            {
                Brand updateBrand = new Brand
                {
                    BrandId = brand.BrandId,
                    BrandName = brand.BrandName,
                    Address = brand.Address,
                    Phone = brand.Phone,
                };

                if (image != null)
                {
                    string imageExtension = Path.GetExtension(image.FileName).ToLower();
                    if (!ImageUtils.CheckImageExtension(imageExtension))
                    {
                        return Ok(new ApiResponse
                        {
                            Status = false,
                            Message = "Hình ảnh hãng không hợp lệ"
                        });
                    }

                    byte[] imageBytes = ConvertFile.ConvertIFormFileToByteArray(image);
                    string path = $"{IMAGE_PATH}{brand.BrandName}{imageExtension}";

                    updateBrand.LogoSrc = await _azureService.UploadImage(path, imageBytes);
                }

                bool result = await _brandService.Update(updateBrand);

                return Ok(new ApiResponse
                {
                    Status = result,
                    Message = result ? "Cập nhật hãng thành công" : "Cập nhật hãng thất bại"
                });
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(string id)
        {
            Brand? brand = await _brandService.GetById(id);
            if (brand == null)
            {
                return Ok(new ApiResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Danh mục không tồn tại"
                });
            }

            bool result = await _brandService.Delete(id);
            if (result && brand.LogoSrc != null)
            {
                await _azureService.DeleteImage(brand.LogoSrc);
            }

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa hãng thành công" : "Xóa hãng thất bại"
            });
        }
    }
}
