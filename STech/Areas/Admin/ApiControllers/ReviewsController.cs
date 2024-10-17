using Microsoft.AspNetCore.Mvc;
using STech.Data.Models;
using STech.Data.MongoViewModels;
using STech.Data.ViewModels;
using STech.Filters;
using STech.Services;
using System.Security.Claims;

namespace STech.Areas.Admin.ApiControllers
{
    [AdminAuthorize]
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IAzureService _azureService;
        private readonly IUserService _userService;

        public ReviewsController(IReviewService reviewService, IAzureService azureService, IUserService userService)
        {
            _reviewService = reviewService;
            _azureService = azureService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetReviews(string? search, string? sort_by, string? status, string? filter_by, int page = 1) {

            if (page <= 1)
            {
                page = 1;
            }

            var (reviews, totalPages) = search != null
                ? await _reviewService.SearchReviewsWithProduct(search, 40, sort_by, status, filter_by, page)
                : await _reviewService.GetReviewsWithProduct(40, sort_by, status, filter_by, page);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = new
                {
                    reviews,
                    totalPages,
                    currentPage = page
                }
            });
        }

        [HttpGet("1/{id}")]
        public async Task<IActionResult> GetReview(int id)
        {
            ReviewMVM? review = await _reviewService.GetReview(id);

            return Ok(new ApiResponse
            {
                Status = true,
                Data = review
            });
        }

        [HttpPatch("approve/{id}")]
        public async Task<IActionResult> ApproveReview(int id, string pId)
        {
            bool result = await _reviewService.ApproveReview(pId, id);

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Phê duyệt đánh giá thành công" : "Không thể phê duyệt đánh giá này"
            });
        }

        [HttpPatch("mark-all-read/{id}")]
        public async Task<IActionResult> MarkAllRepliesAsRead(int id, string pId)
        {
            return Ok(new ApiResponse
            {
                Status = await _reviewService.MarkAllRepliesAsRead(pId, id)
            });
        }

        [HttpPost("post-reply")]
        public async Task<IActionResult> PostReply(ReviewReplyVM reply)
        {
            if(ModelState.IsValid)
            {
                if (reply.Content.Trim().Length <= 0)
                {
                    return BadRequest();
                }

                string? userId = User.FindFirstValue("Id");
                if (userId == null)
                {
                    return Unauthorized();
                }

                User? user = await _userService.GetUserById(userId);
                if (user == null)
                {
                    return Unauthorized();
                }

                ReviewReply newReviewReply = new ReviewReply
                {
                    Content = reply.Content,
                    ReplyDate = DateTime.Now,
                    IsRead = true,
                    UserReplyId = user.UserId
                };

                bool result = await _reviewService.CreateReviewReply(reply.ProductId, reply.ReviewId, newReviewReply);

                return Ok(new ApiResponse
                {
                    Status = result,
                    Data = result ? new ReviewReplyMVM
                    {
                        Content = newReviewReply.Content,
                        UserReply = new User
                        {
                            FullName = user.FullName,
                            Avatar = user.Avatar
                        }
                    } : null
                });
            }

            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id, string pId)
        {
            ReviewMVM? review = await _reviewService.GetReview(id);
            if (review == null)
            {
                return NotFound(new ApiResponse
                {
                    Status = false,
                    StatusCode = 404,
                    Message = "Không tìm thấy đánh giá"
                });
            }

            bool result = await _reviewService.DeleteReview(pId, id);
            if (result)
            {
                foreach(ReviewImage img in review.ReviewImages)
                {
                    await _azureService.DeleteImage(img.ImageUrl);
                }
            }

            return Ok(new ApiResponse
            {
                Status = result,
                Message = result ? "Xóa đánh giá thành công" : "Không thể xóa đánh giá này"
            });
        }
    }
}
