using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Location;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.IValidation;
using OfficeOpenXml;
using System.Linq.Expressions;
using System.Net;

namespace CarpoolPlatformAPI.Services
{
    public class RideService : IRideService
    {
        private readonly IRideRepository _rideRepository;
        private readonly IUserRepository _userRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper; 

        public RideService(IRideRepository rideRepository, IUserRepository userRepository, IBookingRepository bookingRepository,
            ILocationRepository locationRepository, INotificationRepository notificationRepository, IValidationService validationService,
            IMapper mapper)
        {
            _rideRepository = rideRepository;
            _userRepository = userRepository;
            _bookingRepository = bookingRepository;
            _locationRepository = locationRepository;
            _notificationRepository = notificationRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<RideDTO>>> GetAllRidesAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var rides = await _rideRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return new ServiceResponse<List<RideDTO>>(HttpStatusCode.OK, _mapper.Map<List<RideDTO>>(rides));
        }

        public async Task<ServiceResponse<RideDTO?>> GetRideAsync(Expression<Func<Ride, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var ride = await _rideRepository.GetAsync(filter, includeProperties, notTracked);

            if(ride == null)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.NotFound, "The ride has not been found.");
            }

            return new ServiceResponse<RideDTO?>(HttpStatusCode.OK, _mapper.Map<RideDTO>(ride));
        }

        public async Task<ServiceResponse<RideDTO?>> CreateRideAsync(RideCreateDTO rideCreateDTO)
        {
            var ride = _mapper.Map<Ride>(rideCreateDTO);
            ride.CreatedAt = DateTime.Now;
            var user = await _userRepository.GetAsync(u => u.Id == rideCreateDTO.UserId && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            } 
            //else if (_validationService.GetCurrentUserId() != ride.UserId)
            //{
            //    return new ServiceResponse<RideDTO?>(HttpStatusCode.Forbidden, "You are not allowed to post this ride.");
            //}
            else if (rideCreateDTO.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest,
                    "You can post a ride only up to three hours before it is supposed to happen.");
            }
            else if (ride.StartLocation == ride.EndLocation)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest, 
                    "Your ride's start location and end location have to be different.");
            }
            else if (ride.SeatsAvailable == 4 && ride.TwoInBackseat)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest,
                    "There can be only up to three available seats in a ride if the 'two in backseat' option is enabled.");
            }

            user.Rides.Add(ride);
            ride = await _rideRepository.CreateAsync(ride);

            var notification = new Notification
            {
                Message = $"You have successfully posted a new ride, happening on " +
                    $"{ride.DepartureTime.Date}, at {ride.DepartureTime.Hour}:{ride.DepartureTime.Minute}.",
                UserId = user.Id,
                CreatedAt = DateTime.Now
            };
            user.Notifications.Add(notification);
            user.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<RideDTO?>(HttpStatusCode.OK, _mapper.Map<RideDTO>(ride));
        }

        public async Task<ServiceResponse<RideDTO?>> UpdateRideAsync(int id, RideUpdateDTO rideUpdateDTO)
        {
            var ride = await _rideRepository.GetAsync(
                r => r.Id == id && 
                     r.DeletedAt == null,
                     includeProperties: "User, Bookings, Bookings.User");

            if (ride == null)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.NotFound, "The ride has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != ride.UserId)
            //{
            //    return new ServiceResponse<RideDTO?>(HttpStatusCode.Forbidden, "You are not allowed to edit this ride.");
            //}
            else if (ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest, 
                    "You can edit a ride only up to three hours before it happens.");
            } 
            else if (ride.Bookings.Where(b => b.BookingStatus == "accepted").Sum(b => b.SeatsBooked) > rideUpdateDTO.SeatsAvailable)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest, 
                    "You cannot lower the number of available seats because your ride has too many accepted bookings.");
            }
            else if (ride.StartLocation == rideUpdateDTO.EndLocation || ride.EndLocation == rideUpdateDTO.StartLocation ||
                rideUpdateDTO.StartLocation == rideUpdateDTO.EndLocation)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest,
                    "Your ride's start location and end location have to be different.");
            }
            else if (ride.SeatsAvailable == 4 && ride.TwoInBackseat)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest,
                    "There can be only up to three available seats in a ride if the 'two in backseat' option is enabled.");
            }

            _mapper.Map(rideUpdateDTO, ride);
            ride.UpdatedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);

            foreach (var booking in ride.Bookings)
            {
                if (booking.BookingStatus == "accepted" || booking.BookingStatus == "requested")
                {
                    var rideCreator = ride.User!;
                    var notification = new Notification
                    {
                        Message = $"The ride by {rideCreator.FirstName} {rideCreator.LastName}, happening on " +
                            $"{ride.DepartureTime.Date}, at {ride.DepartureTime.Hour}:{ride.DepartureTime.Minute}." +
                                  $" has been updated.",
                        UserId = booking.UserId,
                        CreatedAt = DateTime.Now
                    };
                    booking.User.Notifications.Add(notification);
                    booking.User.UpdatedAt = DateTime.Now;
                    await _notificationRepository.CreateAsync(notification);
                }
            }

            return new ServiceResponse<RideDTO?>(HttpStatusCode.OK, _mapper.Map<RideDTO>(ride));
        }

        public async Task<ServiceResponse<RideDTO?>> RemoveRideAsync(int id)
        {
            var ride = await _rideRepository.GetAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "User, Bookings, Bookings.User");

            if (ride == null)
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.NotFound, "The ride has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != ride.UserId)
            //{
            //    return new ServiceResponse<RideDTO?>(HttpStatusCode.Forbidden, "You are not allowed to remove this ride.");
            //}
            else if (ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<RideDTO?>(HttpStatusCode.BadRequest, 
                    "You can remove a ride only up to three hours before it happens.");
            }

            ride.DeletedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);
            var rideCreator = ride.User;

            foreach (var booking in ride.Bookings)
            {
                if (booking.BookingStatus == "accepted" || booking.BookingStatus == "requested")
                {
                    booking.BookingStatus = "cancelled";
                    booking.UpdatedAt = DateTime.Now;
                    var notification = new Notification
                    {
                        Message = $"The ride by {rideCreator.FirstName} {rideCreator.LastName}, happening on " +
                            $"{ride.DepartureTime.Date}, at  {ride.DepartureTime.Hour}:{ride.DepartureTime.Minute}, has been cancelled.",
                        UserId = booking.UserId,
                        CreatedAt = DateTime.Now
                    };
                    booking.User.Notifications.Add(notification);
                    booking.User.UpdatedAt = DateTime.Now;
                    await _notificationRepository.CreateAsync(notification);
                }
            }

            return new ServiceResponse<RideDTO?>(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResponse<List<LocationDTO>>> GetAllLocationsAsync(Expression<Func<Location, bool>>? filter = null)
        {
            var locations = await _locationRepository.GetAllAsync(filter);

            return new ServiceResponse<List<LocationDTO>>(HttpStatusCode.OK, _mapper.Map<List<LocationDTO>>(locations));
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
    }
}
