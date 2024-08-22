using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services;
using CarpoolPlatformAPI.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Route("received/{id}")]
        public async Task<IActionResult> GetAllReceivedReviewsForUser([FromRoute] string userId)
        {
            var reviewDTOs = await _reviewService.GetAllReviewsAsync(
                r => r.RevieweeId == userId &&
                r.DeletedAt == null,
                includeProperties: "User, User.Picture");

            return Ok(reviewDTOs);
        }

        [HttpGet]
        [Route("given/{id}")]
        public async Task<IActionResult> GetAllGivenReviewsForUser([FromRoute] string userId)
        {
            var reviewDTOs = await _reviewService.GetAllReviewsAsync(
                r => r.ReviewerId == userId &&
                r.DeletedAt == null,
                includeProperties: "User, User.Picture");

            return Ok(reviewDTOs);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetReviewById([FromRoute] int id)
        {
            var reviewDTO = await _reviewService.GetReviewAsync(
                r => r.Id == id,
                includeProperties: "User, User.Picture");

            if (reviewDTO == null)
            {
                return NotFound(new { message = "The review has not been found." });
            }

            return Ok(reviewDTO);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDTO reviewCreateDTO)
        {
            var reviewDTO = await _reviewService.CreateReviewAsync(reviewCreateDTO);

            return CreatedAtAction(nameof(GetReviewById), new { id = reviewDTO.Id }, reviewDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] ReviewUpdateDTO reviewUpdateDTO)
        {
            var reviewDTO = await _reviewService.UpdateReviewAsync(id, reviewUpdateDTO);

            if (reviewDTO == null)
            {
                return NotFound(new { message = "The review has not been found." });
            }

            return Ok(reviewDTO);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            var reviewDTO = await _reviewService.RemoveReviewAsync(id);

            if (reviewDTO == null)
            {
                return NotFound(new { message = "The review has not been found." });
            }

            return Ok(reviewDTO);
        }
    }
}
