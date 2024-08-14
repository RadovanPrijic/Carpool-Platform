using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class RideService : IRideService
    {
        public Task<RideDTO> CreateRideAsync(RideCreateDTO rideCreateDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<RideDTO>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public Task<RideDTO?> GetRideAsync(Expression<Func<Ride, bool>> filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<RideDTO?> RemoveRideAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<RideDTO?> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
