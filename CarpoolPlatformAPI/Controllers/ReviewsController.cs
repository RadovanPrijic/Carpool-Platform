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
        public async Task<IActionResult> GetAllReviews()
        {
            var reviewDTOs = await _reviewService.GetAllReviewsAsync();  // Include necessary props here

            return Ok(reviewDTOs);
        }

        [HttpGet]
        [Route("{id:string}")]
        public async Task<IActionResult> GetReviewById([FromRoute] int id)
        {
            var reviewDTO = await _reviewService.GetReviewAsync(r => r.Id == id); // Include necessary props here

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
        [Route("{id:string}")]
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
        [Route("{id:string}")]
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
