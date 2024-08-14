using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Ride;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IRideService
    {
        Task<List<RideDTO>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1);
        Task<RideDTO?> GetRideAsync(Expression<Func<Ride, bool>> filter = null, string? includeProperties = null);
        Task<RideDTO> CreateRideAsync(RideCreateDTO rideCreateDTO);
        Task<RideDTO?> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO);
        Task<RideDTO?> RemoveRideAsync(int id);
    }
}
