using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IUserRepository userRepository, IRideRepository rideRepository,
            IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _userRepository = userRepository;
            _rideRepository = rideRepository;
            _mapper = mapper;
        }

        public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
        {
            var booking = _mapper.Map<Booking>(bookingCreateDTO);
            booking.BookingStatus = "requested";
            booking.CreatedAt = DateTime.Now;

            var user = await _userRepository.GetAsync(u => u.Id == bookingCreateDTO.UserId, includeProperties: "Bookings");
            var ride = await _rideRepository.GetAsync(r => r.Id == bookingCreateDTO.RideId, includeProperties: "Bookings");

            if (user == null)
            {
                // TODO Throw Error("The associated user has not been found.")
            } else if (ride == null)
            {
                // TODO Throw Error("The associated ride has not been found.")
            }

            user!.Bookings.Add(booking);
            user.UpdatedAt = DateTime.Now;

            ride!.Bookings.Add(booking);
            ride.UpdatedAt = DateTime.Now;

            booking = await _bookingRepository.CreateAsync(booking);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<List<BookingDTO>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var bookings = await _bookingRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);
            
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO?> GetBookingAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var booking = await _bookingRepository.GetAsync(filter, includeProperties, notTracked);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO?> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO)
        {
            var booking = await _bookingRepository.GetAsync(b => b.Id == id && b.DeletedAt == null);

            if (booking == null)
            {
                return null;
            }

            //TODO Ovde se zapravo radi azuriranje statusa Bookinga i potrebno je slati notifikacije

            booking = _mapper.Map<Booking>(bookingUpdateDTO);
            booking.UpdatedAt = DateTime.Now;
            booking = await _bookingRepository.UpdateAsync(booking);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO?> RemoveBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetAsync(
                b => b.Id == id && 
                b.BookingStatus == "requested" &&
                b.DeletedAt == null,
                includeProperties: "User, User.Bookings, Ride, Ride.Bookings");

            if (booking == null)
            {
                return null;
            }

            booking.User.Bookings.Remove(booking);
            booking.User.UpdatedAt = DateTime.Now;

            booking.Ride.Bookings.Remove(booking);
            booking.Ride.UpdatedAt = DateTime.Now;

            booking.DeletedAt = DateTime.Now;
            booking = await _bookingRepository.UpdateAsync(booking);

            return _mapper.Map<BookingDTO>(booking);

            /*

            foreach (var booking in ride.Bookings)
            {
                booking.BookingStatus = "cancelled";
                booking.UpdatedAt = DateTime.Now;
            }

            ride.User.UpdatedAt = DateTime.Now;
            ride.DeletedAt = DateTime.Now;
            ride = await _rideRepository.UpdateAsync(ride);

            return _mapper.Map<RideDTO>(ride);
             */
        }
    }
}
