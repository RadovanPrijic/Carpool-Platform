using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;
        private readonly IMapper _mapper;

        public RideService(IRideRepository rideRepository, IMapper mapper)
        {
            _rideRepository = rideRepository;
            _mapper = mapper;
        }

        public async Task<RideDTO> CreateRideAsync(RideCreateDTO rideCreateDTO)
        {
            var ride = await _rideRepository.CreateAsync(_mapper.Map<Ride>(rideCreateDTO));

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<List<RideDTO>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            var rides = await _rideRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber);

            return _mapper.Map<List<RideDTO>>(rides);
        }

        public async Task<RideDTO?> GetRideAsync(Expression<Func<Ride, bool>> filter = null, string? includeProperties = null)
        {
            var ride = await _rideRepository.GetAsync(filter, includeProperties);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<RideDTO?> RemoveRideAsync(int id)
        {
            var ride = await _rideRepository.GetAsync(r => r.Id == id);

            if (ride == null)
            {
                return null;
            }

            ride.DeletedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<RideDTO?> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO)
        {
            var ride = await _rideRepository.GetAsync(r => r.Id == id);

            if (ride == null)
            {
                return null;
            }

            ride = await _rideRepository.UpdateAsync(_mapper.Map<Ride>(rideUpdateDTO));

            return _mapper.Map<RideDTO>(ride);
        }
    }
}
