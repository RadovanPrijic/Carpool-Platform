using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private APIResponse _response;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
            _response = new();
        }

        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllBookingsForUser([FromRoute] string userId)
        {
            var bookingDTOs = await _bookingService.GetAllBookingsAsync(
                b => b.UserId == userId &&
                b.DeletedAt == null,
                includeProperties: "Ride, Review");

            return Ok(bookingDTOs);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetBookingById([FromRoute] int id)
        {
            var bookingDTO = await _bookingService.GetBookingAsync(
                b => b.Id == id &&
                b.DeletedAt == null,
                includeProperties: "Ride, Review");

            if (bookingDTO == null)
            {
                return NotFound(new { message = "The booking has not been found." });
            }

            return Ok(bookingDTO);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingCreateDTO)
        {
            var bookingDTO = await _bookingService.CreateBookingAsync(bookingCreateDTO);

            return CreatedAtAction(nameof(GetBookingById), new { id = bookingDTO.Id }, bookingDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateBooking([FromRoute] int id, [FromBody] BookingUpdateDTO bookingUpdateDTO)
        {
            var bookingDTO = await _bookingService.UpdateBookingAsync(id, bookingUpdateDTO);

            if (bookingDTO == null)
            {
                return NotFound(new { message = "The booking has not been found." });
            }

            return Ok(bookingDTO);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteBooking([FromRoute] int id)
        {
            var bookingDTO = await _bookingService.RemoveBookingAsync(id);

            if (bookingDTO == null)
            {
                return NotFound(new { message = "The booking has not been found." });
            }

            return Ok(bookingDTO);
        }
    }
}
