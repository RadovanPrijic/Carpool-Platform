using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RidesController : ControllerBase
    {
        private readonly IRideService _rideService;

        public RidesController(IRideService rideService)
        {
            _rideService = rideService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFilteredRides(
            [FromQuery] string from,
            [FromQuery] string to,
            [FromQuery] string date,
            [FromQuery] int seats)
        {
            string dateFormat = "yyyy-MM-dd";
            DateTime dateTime = DateTime.ParseExact(date, dateFormat, CultureInfo.InvariantCulture);

            var serviceResponse = await _rideService.GetAllRidesAsync(
                r => r.StartLocation == from &&
                     r.EndLocation == to &&
                     r.DepartureTime.Date == dateTime.Date &&
                     r.SeatsAvailable >= seats &&
                     r.DeletedAt == null, 
                     includeProperties: "User, User.Picture, Bookings");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllRidesForUser([FromRoute] string id)
        {
            var serviceResponse = await _rideService.GetAllRidesAsync(
                r => r.UserId == id &&
                     r.DeletedAt == null,
                     includeProperties: "User, User.Picture, Bookings");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetRideById([FromRoute] int id)
        {
            var serviceResponse = await _rideService.GetRideAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "User, User.Picture, Bookings");
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateRide([FromBody] RideCreateDTO rideCreateDTO)
        {
            var serviceResponse = await _rideService.CreateRideAsync(rideCreateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse, this, nameof(GetRideById), new { id = serviceResponse.Data!.Id });
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRide([FromRoute] int id, [FromBody] RideUpdateDTO rideUpdateDTO)
        {
            var serviceResponse = await _rideService.UpdateRideAsync(id, rideUpdateDTO);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteRide([FromRoute] int id)
        {
            var serviceResponse = await _rideService.RemoveRideAsync(id);
            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("locations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var serviceResponse = await _rideService.GetAllLocationsAsync();
            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
