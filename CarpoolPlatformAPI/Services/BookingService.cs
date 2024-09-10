using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.IValidation;
using System.Linq.Expressions;
using System.Net;

namespace CarpoolPlatformAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IUserRepository userRepository, IRideRepository rideRepository,
            INotificationRepository notificationRepository, IValidationService validationService, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _rideRepository = rideRepository;
            _notificationRepository = notificationRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<BookingDTO>>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var bookings = await _bookingRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return new ServiceResponse<List<BookingDTO>>(HttpStatusCode.OK, _mapper.Map<List<BookingDTO>>(bookings));
        }

        public async Task<ServiceResponse<BookingDTO?>> GetBookingAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var booking = await _bookingRepository.GetAsync(filter, includeProperties, notTracked);

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The booking has not been found.");
            }

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.OK, _mapper.Map<BookingDTO>(booking));
        }

        public async Task<ServiceResponse<BookingDTO?>> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
        {
            var booking = _mapper.Map<Booking>(bookingCreateDTO);
            booking.BookingStatus = "requested";
            booking.CreatedAt = DateTime.Now;
            var user = await _userRepository.GetAsync(u => u.Id == bookingCreateDTO.UserId && u.DeletedAt == null);
            var ride = await _rideRepository.GetAsync(r => r.Id == bookingCreateDTO.RideId && r.DeletedAt == null,
                includeProperties: "User, Bookings");

            if (user == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            } 
            else if (ride == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The ride has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != booking.UserId)
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.Forbidden, "You are not allowed to post this booking.");
            //}
            //else if (ride.UserId == booking.UserId)
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest, "You are not allowed to book your own ride.");
            //}
            //else if (ride.DepartureTime < DateTime.Now.AddHours(3))
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest,
            //        "You can make a booking only up to three hours before the ride.");
            //}
            //else if (ride.Bookings.Where(b => b.BookingStatus == "accepted").Sum(b => b.SeatsBooked) + booking.SeatsBooked
            //         > ride.SeatsAvailable)
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest,
            //        "This ride has already filled its maximum capacity.");
            //}

            if (ride.AutomaticBooking)
            {
                booking.BookingStatus = "accepted";
            }

            user.Bookings.Add(booking);
            user.UpdatedAt = DateTime.Now;

            ride.Bookings.Add(booking);
            ride.UpdatedAt = DateTime.Now;

            booking = await _bookingRepository.CreateAsync(booking);

            var rideCreator = ride.User;
            var notification = new Notification
            {
                Message = 
                    $"{( !ride.AutomaticBooking ? 
                    $"{user.FirstName} {user.LastName} has requested to book your ride," :
                    $"{user.FirstName} {user.LastName} has booked your ride," )}" +
                    $" happening on {ride.DepartureTime.Date}, at {ride.DepartureTime.Hour}:{ride.DepartureTime.Minute}.",
                UserId = rideCreator.Id,
                CreatedAt = DateTime.Now
            };
            rideCreator.Notifications.Add(notification);
            rideCreator.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.OK, _mapper.Map<BookingDTO>(booking));
        }

        public async Task<ServiceResponse<BookingDTO?>> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO)
        {
            var booking = await _bookingRepository.GetAsync(
                b => b.Id == id &&
                     (b.BookingStatus == "requested" || b.BookingStatus == "accepted") &&
                     b.DeletedAt == null,
                     includeProperties: "User, Ride, Ride.User, Ride.Bookings");

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The booking has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != booking.Ride.UserId)
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.Forbidden, "You are not allowed to accept or reject this booking.");
            //}
            else if (booking.Ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest,
                    "You can accept or reject a booking only up to three hours before the ride.");
            }
            else if ((booking.Ride.Bookings.Where(b => b.BookingStatus == "accepted").Sum(b => b.SeatsBooked) + booking.SeatsBooked
                     > booking.Ride.SeatsAvailable) && bookingUpdateDTO.BookingStatus == "accepted")
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest, 
                    "Accepting this booking would exceed the maximum number of booked seats for this ride.");
            }

            _mapper.Map(bookingUpdateDTO, booking);
            booking.UpdatedAt = DateTime.Now;
            booking = await _bookingRepository.UpdateAsync(booking);

            var rideCreator = booking.Ride.User;
            var rideDateTime = booking.Ride.DepartureTime;
            var notification = new Notification
            {
                Message = $"{rideCreator.FirstName} {rideCreator.LastName} has {booking.BookingStatus} your booking for their ride," +
                          $" happening on {rideDateTime.Date}, at {rideDateTime.Hour}:{rideDateTime.Minute}.",
                UserId = booking.User.Id,
                CreatedAt = DateTime.Now
            };
            booking.User.Notifications.Add(notification);
            booking.User.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.OK, _mapper.Map<BookingDTO>(booking));
        }

        public async Task<ServiceResponse<BookingDTO?>> CancelBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetAsync(
                b => b.Id == id && 
                     (b.BookingStatus == "requested" || b.BookingStatus == "accepted") &&
                     b.DeletedAt == null,
                     includeProperties: "User, Ride, Ride.User");

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The booking has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != booking.UserId ||
            //         _validationService.GetCurrentUserId() != booking.Ride.UserId)
            //{
            //    return new ServiceResponse<BookingDTO?>(HttpStatusCode.Forbidden, "You are not allowed to cancel this booking.");
            //}
            else if (booking.Ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest, 
                    "You can cancel a booking only up to three hours before the ride.");
            }

            booking.BookingStatus = "cancelled";
            booking = await _bookingRepository.UpdateAsync(booking);

            var rideCreator = booking.Ride.User;
            var rideDateTime = booking.Ride.DepartureTime;
            var notification = new Notification
            {
                Message = $"{booking.User.FirstName} {booking.User.LastName} has cancelled their booking for your ride, happening on." +
                    $"{rideDateTime.Date}, at {rideDateTime.Hour}:{rideDateTime.Minute}.",
                UserId = rideCreator.Id,
                CreatedAt = DateTime.Now
            };
            rideCreator.Notifications.Add(notification);
            rideCreator.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.OK, _mapper.Map<BookingDTO>(booking));
        }
    }
}
