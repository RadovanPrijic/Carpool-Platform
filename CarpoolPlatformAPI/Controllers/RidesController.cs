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
        public async Task<IActionResult> GetAllRides()
        {
            var rideDTOs = await _rideService.GetAllRidesAsync();  // Include necessary props here

            return Ok(rideDTOs);
        }

        [HttpGet]
        [Route("{id:string}")]
        public async Task<IActionResult> GetRideById([FromRoute] int id)
        {
            var rideDTO = await _rideService.GetRideAsync(r => r.Id == id); // Include necessary props here

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
        [Route("{id:string}")]
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
        [Route("{id:string}")]
        public async Task<IActionResult> DeleteRide([FromRoute] int id)
        {
            var rideDTO = await _rideService.RemoveRideAsync(id);

            if (rideDTO == null)
            {
                return NotFound(new { message = "The ride has not been found." });
            }

            return Ok(rideDTO);
        }
    }
}
