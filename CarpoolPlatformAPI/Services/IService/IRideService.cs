using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Location;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IRideService
    {
        Task<ServiceResponse<List<RideDTO>>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<ServiceResponse<RideDTO?>> GetRideAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<ServiceResponse<RideDTO?>> CreateRideAsync(RideCreateDTO rideCreateDTO);
        Task<ServiceResponse<RideDTO?>> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO);
        Task<ServiceResponse<RideDTO?>> RemoveRideAsync(int id);
        Task<ServiceResponse<List<LocationDTO>>> GetAllLocationsAsync(Expression<Func<Location, bool>>? filter = null);
        Task ImportLocationsFromExcelAsync(string filePath);
    }
}
