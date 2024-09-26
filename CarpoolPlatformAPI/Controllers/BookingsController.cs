using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        [Route("filtered/{id}")]
        public async Task<IActionResult> GetFilteredBookings([FromRoute] string id, [FromQuery] string filter)
        {
            string includeProperties = "User, Ride, Ride.User, Review";
            ServiceResponse<List<BookingDTO>> serviceResponse = filter switch
            {
                "by-user-requested" => await _bookingService.GetAllBookingsAsync(
                                             b => b.UserId == id &&
                                                  b.BookingStatus == "requested" &&
                                                  b.DeletedAt == null,
                                                  includeProperties),
                "by-user-accepted" => await _bookingService.GetAllBookingsAsync(
                                             b => b.UserId == id &&
                                                  b.BookingStatus == "accepted" &&
                                                  b.DeletedAt == null,
                                                  includeProperties),
                "for-user-requested" => await _bookingService.GetAllBookingsAsync(
                                             b => b.Ride.UserId == id &&
                                                  b.BookingStatus == "requested" &&
                                                  b.DeletedAt == null,
                                                  includeProperties),
                "for-user-accepted" => await _bookingService.GetAllBookingsAsync(
                                             b => b.Ride.UserId == id &&
                                                  b.BookingStatus == "accepted" &&
                                                  b.DeletedAt == null,
                                                  includeProperties),
                _ => new ServiceResponse<List<BookingDTO>>(HttpStatusCode.OK, [])
            };
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("for-ride/{id:int}")]
        public async Task<IActionResult> GetAllBookingsForRide([FromRoute] int id)
        {
            var serviceResponse = await _bookingService.GetAllBookingsAsync(
                b => b.Ride.Id == id &&
                     b.DeletedAt == null,
                     includeProperties: "User, Ride, Ride.User, Review");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetBookingById([FromRoute] int id)
        {
            var serviceResponse = await _bookingService.GetBookingAsync(
                b => b.Id == id &&
                     b.DeletedAt == null,
                     includeProperties: "Ride, Review");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDTO bookingCreateDTO)
        {
            var serviceResponse = await _bookingService.CreateBookingAsync(bookingCreateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateBooking([FromRoute] int id, [FromBody] BookingUpdateDTO bookingUpdateDTO)
        {
            var serviceResponse = await _bookingService.UpdateBookingAsync(id, bookingUpdateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> CancelBooking([FromRoute] int id)
        {
            var serviceResponse = await _bookingService.CancelBookingAsync(id);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
