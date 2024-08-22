using AutoMapper;
using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SqlServer.Server;
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

            var rideDTOs = await _rideService.GetAllRidesAsync(
                r => r.StartLocation == from &&
                     r.EndLocation == to &&
                     r.DepartureTime.Date == dateTime.Date &&
                     r.SeatsAvailable >= seats &&
                     r.DeletedAt == null, 
                     includeProperties: "User, User.Picture");

            return Ok(rideDTOs);
        }

        [HttpGet]
        [Route("all/{id}")]
        public async Task<IActionResult> GetAllRidesForUser([FromRoute] string userId)
        {
            var rideDTOs = await _rideService.GetAllRidesAsync(
                r => r.UserId == userId &&
                     r.DeletedAt == null); 

            return Ok(rideDTOs);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetRideById([FromRoute] int id)
        {
            var rideDTO = await _rideService.GetRideAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "User, User.Picture");

            if (rideDTO == null)
            {
                return NotFound(new { message = "The ride has not been found." });
            }

            return Ok(rideDTO);
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateRide([FromBody] RideCreateDTO rideCreateDTO)
        {
            var rideDTO = await _rideService.CreateRideAsync(rideCreateDTO);

            return CreatedAtAction(nameof(GetRideById), new { id = rideDTO.Id }, rideDTO);
        }

        [HttpPut]
        [Route("{id:int}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateRide([FromRoute] int id, [FromBody] RideUpdateDTO rideUpdateDTO)
        {
            var rideDTO = await _rideService.UpdateRideAsync(id, rideUpdateDTO);
            
            if (rideDTO == null)
            {
                return NotFound(new { message = "The ride has not been found." });
            }

            return Ok(rideDTO);
        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteRide([FromRoute] int id)
        {
            var rideDTO = await _rideService.RemoveRideAsync(id);

            if (rideDTO == null)
            {
                return NotFound(new { message = "The ride has not been found." });
            }

            return Ok(rideDTO);
        }

        [HttpGet]
        [Route("locations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var locationDTOs = await _rideService.GetAllLocationsAsync();

            return Ok(locationDTOs);
        }
    }
}
