using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IBookingService
    {
        Task<List<BookingDTO>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<BookingDTO?> GetBookingAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<BookingDTO> CreateBookingAsync(BookingCreateDTO bookingCreateDTO);
        Task<BookingDTO?> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO);
        Task<BookingDTO?> RemoveBookingAsync(int id);
    }
}
