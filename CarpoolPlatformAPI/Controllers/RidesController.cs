using AutoMapper;
using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RidesController : ControllerBase
    {
        private readonly IRideService _rideService;
        private readonly IValidationService _validationService;

        public RidesController(IRideService rideService, IValidationService validationService)
        {
            _rideService = rideService;
            _validationService = validationService;
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

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllRidesForUser([FromRoute] string userId)
        {
            var serviceResponse = await _rideService.GetAllRidesAsync(
                r => r.UserId == userId &&
                     r.DeletedAt == null,
                     includeProperties: "User, User.Picture, Bookings");

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetRideById([FromRoute] int id)
        {
            var serviceResponse = await _rideService.GetRideAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "User, User.Picture, Bookings");

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                case HttpStatusCode.NotFound:
                    return NotFound(new { message = serviceResponse.ErrorMessage });
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateRide([FromBody] RideCreateDTO rideCreateDTO)
        {
            var serviceResponse = await _rideService.CreateRideAsync(rideCreateDTO);

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.Created:
                    return CreatedAtAction(nameof(GetRideById), new { id = serviceResponse.Data!.Id }, serviceResponse.Data);
                case HttpStatusCode.NotFound:
                    return NotFound(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.BadRequest:
                    return BadRequest(new { message = serviceResponse.ErrorMessage });
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRide([FromRoute] int id, [FromBody] RideUpdateDTO rideUpdateDTO)
        {
            var serviceResponse = await _rideService.UpdateRideAsync(id, rideUpdateDTO);

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                case HttpStatusCode.NotFound:
                    return NotFound(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.BadRequest:
                    return BadRequest(new { message = serviceResponse.ErrorMessage });
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteRide([FromRoute] int id)
        {
            var serviceResponse = await _rideService.RemoveRideAsync(id);

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                case HttpStatusCode.NotFound:
                    return NotFound(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.Unauthorized:
                    return Unauthorized(new { message = serviceResponse.ErrorMessage });
                case HttpStatusCode.BadRequest:
                    return BadRequest(new { message = serviceResponse.ErrorMessage });
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpGet]
        [Route("locations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var serviceResponse = await _rideService.GetAllLocationsAsync();

            switch (serviceResponse.StatusCode)
            {
                case HttpStatusCode.OK:
                    return Ok(serviceResponse.Data);
                default:
                    return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
