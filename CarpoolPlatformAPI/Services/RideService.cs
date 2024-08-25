using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Location;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Drawing.Printing;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace CarpoolPlatformAPI.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public RideService(IRideRepository rideRepository, IUserRepository userRepository, IBookingRepository bookingRepository,
            ILocationRepository locationRepository, IMapper mapper, IValidationService validationService)
        {
            _rideRepository = rideRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _locationRepository = locationRepository;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<RideDTO> CreateRideAsync(RideCreateDTO rideCreateDTO)
        {
            var ride = _mapper.Map<Ride>(rideCreateDTO);
            ride.CreatedAt = DateTime.Now;

            var user = await _userRepository.GetAsync(u => u.Id == rideCreateDTO.UserId, includeProperties: "Rides");

            if(user == null)
            {
                // TODO Throw Error("User has not been found.")
            }

            user!.Rides.Add(ride);
            user.UpdatedAt = DateTime.Now;

            ride = await _rideRepository.CreateAsync(ride);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<List<RideDTO>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var rides = await _rideRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return _mapper.Map<List<RideDTO>>(rides);
        }

        public async Task<RideDTO?> GetRideAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var ride = await _rideRepository.GetAsync(filter, includeProperties, notTracked);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<RideDTO?> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO)
        {
            var ride = await _rideRepository.GetAsync(r => r.Id == id && r.DeletedAt == null);

            if (ride == null)
            {
                return null;
            }

            ride = _mapper.Map<Ride>(rideUpdateDTO);
            ride.UpdatedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task<RideDTO?> RemoveRideAsync(int id)
        {
            var ride = await _rideRepository.GetAsync(
                r => r.Id == id &&
                r.DepartureTime.Date > DateTime.Now &&
                r.DeletedAt == null,
                includeProperties: "User, Bookings");

            if (ride == null)
            {
                return null;
            }

            foreach (var booking in ride.Bookings)
            {
                booking.BookingStatus = "cancelled";
                booking.UpdatedAt = DateTime.Now;
            }

            ride.User.Rides.Remove(ride);
            ride.User.UpdatedAt = DateTime.Now;

            ride.DeletedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);

            return _mapper.Map<RideDTO>(ride);
        }

        public async Task ImportLocationsFromExcelAsync(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var existingLocations = await _locationRepository.GetAllAsync();

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;
                var newLocations = new List<Location>();

                for (int row = 2; row <= rowCount; row++) 
                {
                    var city = worksheet.Cells[row, 1].Value?.ToString()!.Trim();
                    var country = worksheet.Cells[row, 4].Value?.ToString()!.Trim();

                    if (!existingLocations.Any(l => l.City == city && l.Country == country))
                    {
                        var location = new Location
                        {
                            City = city!,
                            Country = country!,
                            CreatedAt = DateTime.Now
                        };

                        newLocations.Add(location);
                    }
                }

                if (newLocations.Count > 0)
                {
                    await _locationRepository.AddLocationsAsync(newLocations);
                }
            }
        }

        public async Task<List<LocationDTO>> GetAllLocationsAsync(Expression<Func<Location, bool>>? filter = null)
        {
            var locations = await _locationRepository.GetAllAsync(filter);

            return _mapper.Map<List<LocationDTO>>(locations);
        }
    }
}
