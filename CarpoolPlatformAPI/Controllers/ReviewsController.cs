using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        [Route("received/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllReceivedReviewsForUser([FromRoute] string id)
        {
            var serviceResponse = await _reviewService.GetAllReviewsAsync(
                r => r.RevieweeId == id &&
                     r.DeletedAt == null,
                     includeProperties: "Reviewer, Reviewer.Picture, Ride, Ride.User");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("given/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllGivenReviewsForUser([FromRoute] string id)
        {
            var serviceResponse = await _reviewService.GetAllReviewsAsync(
                r => r.ReviewerId == id &&
                     r.DeletedAt == null,
                     includeProperties: "Reviewer, Reviewer.Picture, Ride, Ride.User");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetReviewById([FromRoute] int id)
        {
            var serviceResponse = await _reviewService.GetReviewAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "Reviewer, Reviewer.Picture, Ride, Ride.User");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDTO reviewCreateDTO)
        {
            var serviceResponse = await _reviewService.CreateReviewAsync(reviewCreateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateReview([FromRoute] int id, [FromBody] ReviewUpdateDTO reviewUpdateDTO)
        {
            var serviceResponse = await _reviewService.UpdateReviewAsync(id, reviewUpdateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteReview([FromRoute] int id)
        {
            var serviceResponse = await _reviewService.RemoveReviewAsync(id);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
