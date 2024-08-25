using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly IValidationService _validationService;

        public ReviewsController(IReviewService reviewService, IValidationService validationService)
        {
            _reviewService = reviewService;
            _validationService = validationService;
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
            if (_validationService.GetCurrentUserId() != reviewCreateDTO.ReviewerId)
            {
                return Unauthorized(new { message = "You are not authorized to post this review." });
            }

            var reviewDTO = await _reviewService.CreateReviewAsync(reviewCreateDTO);

            if (reviewDTO == null)
            {
                return BadRequest(new { message = "The provided user and/or ride and/or booking data is invalid." });
            }

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
