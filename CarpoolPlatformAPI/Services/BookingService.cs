using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class BookingService : IBookingService
    {
        public Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<BookingDTO>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public Task<BookingDTO?> GetBookingAsync(Expression<Func<Booking, bool>> filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<BookingDTO?> RemoveBookingAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<BookingDTO?> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
