﻿using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
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
            var user = await _userRepository.GetAsync(u => u.Id == bookingCreateDTO.UserId);
            var ride = await _rideRepository.GetAsync(r => r.Id == bookingCreateDTO.RideId, includeProperties: "User");

            if (user == null)
            {
                // TODO Throw Error("The associated user has not been found.")
            } else if (ride == null)
            {
                // TODO Throw Error("The associated ride has not been found.")
            }



            booking.BookingStatus = "requested";
            booking.CreatedAt = DateTime.Now;

            user.Bookings.Add(booking);
            user.UpdatedAt = DateTime.Now;

            ride.Bookings.Add(booking);
            ride.UpdatedAt = DateTime.Now;

            booking = await _bookingRepository.CreateAsync(booking);

            if (!ride.AutomaticBooking)
            {

            }

            var notification = new Notification
            {
                Message = $"{booking.User.FirstName} {booking.User.LastName} has requested to book your ride.",
                UserId = ride.User.Id,
                CreatedAt = DateTime.Now
            };
            user.Notifications.Add(notification);
            user.UpdatedAt = DateTime.Now;

            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.Created, _mapper.Map<BookingDTO>(booking));
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
            else if (_validationService.GetCurrentUserId() != booking.Ride.UserId)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.Unauthorized, "You are not authorized to update this booking.");
            }
            else if (booking.Ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest,
                    "You can accept or reject a booking only up to three hours before the ride.");
            }
            else if (booking.Ride.Bookings.Sum(b => b.SeatsBooked) == booking.Ride.SeatsAvailable)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest, "All seats for this ride have already been booked.");
            }

            var rideCreator = booking.Ride.User;

            if (booking.BookingStatus == "accepted")
            {
                rideCreator.Bookings.Add(booking);
            }

            booking = _mapper.Map<Booking>(bookingUpdateDTO);
            booking.UpdatedAt = DateTime.Now;
            booking = await _bookingRepository.UpdateAsync(booking);

            var notification = new Notification
            {
                Message = $"{rideCreator.FirstName} {rideCreator.LastName} has {booking.BookingStatus} your booking.",
                UserId = booking.User.Id,
                CreatedAt = DateTime.Now
            };
            booking.User.Notifications.Add(notification);
            booking.User.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<BookingDTO?>(HttpStatusCode.OK, _mapper.Map<BookingDTO>(booking));
        }

        public async Task<ServiceResponse<BookingDTO?>> RemoveBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetAsync(
                b => b.Id == id && 
                     (b.BookingStatus == "requested" || b.BookingStatus == "accepted") &&
                     b.DeletedAt == null,
                     includeProperties: "User, User.Bookings, Ride, Ride.User, Ride.Bookings");

            if (booking == null)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.NotFound, "The booking has not been found.");
            }
            else if (_validationService.GetCurrentUserId() != booking.UserId)
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.Unauthorized, "You are not authorized to cancel this booking.");
            }
            else if (booking.BookingStatus == "accepted" && booking.Ride.DepartureTime < DateTime.Now.AddHours(3))
            {
                return new ServiceResponse<BookingDTO?>(HttpStatusCode.BadRequest, 
                    "You can cancel a booking only up to three hours before the ride.");
            }

            booking.User.Bookings.Remove(booking);
            booking.User.UpdatedAt = DateTime.Now;

            booking.Ride.Bookings.Remove(booking);
            booking.Ride.UpdatedAt = DateTime.Now;

            //booking.DeletedAt = DateTime.Now;
            booking.BookingStatus = "cancelled";
            booking = await _bookingRepository.UpdateAsync(booking);

            var rideCreator = booking.Ride.User;
            var notification = new Notification
            {
                Message = $"{booking.User.FirstName} {booking.User.LastName} has cancelled their booking for your ride.",
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
