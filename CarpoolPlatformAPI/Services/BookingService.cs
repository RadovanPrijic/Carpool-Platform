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
        private readonly IMapper _mapper;

        public BookingService(IBookingRepository bookingRepository, IMapper mapper)
        {
            _bookingRepository = bookingRepository;
            _mapper = mapper;
        }

        public async Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
        {
            var booking = await _bookingRepository.CreateAsync(_mapper.Map<Booking>(bookingCreateDTO));

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<List<BookingDTO>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            var bookings = await _bookingRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber);
            
            return _mapper.Map<List<BookingDTO>>(bookings);
        }

        public async Task<BookingDTO?> GetBookingAsync(Expression<Func<Booking, bool>> filter = null, string? includeProperties = null)
        {
            var booking = await _bookingRepository.GetAsync(filter, includeProperties);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO?> RemoveBookingAsync(int id)
        {
            var booking = await _bookingRepository.GetAsync(b => b.Id == id);

            if (booking == null)
            {
                return null;
            }

            booking.DeletedAt = DateTime.Now;
            booking = await _bookingRepository.UpdateAsync(booking);

            return _mapper.Map<BookingDTO>(booking);
        }

        public async Task<BookingDTO?> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO)
        {
            var booking = await _bookingRepository.GetAsync(b => b.Id == id);

            if (booking == null)
            {
                return null;
            }

            booking = await _bookingRepository.UpdateAsync(_mapper.Map<Booking>(bookingUpdateDTO));

            return _mapper.Map<BookingDTO>(booking);
        }
    }
}
