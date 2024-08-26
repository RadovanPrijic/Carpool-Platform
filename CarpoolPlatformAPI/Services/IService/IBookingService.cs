using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IBookingService
    {
        Task<ServiceResponse<List<BookingDTO>>> GetAllBookingsAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<ServiceResponse<BookingDTO?>> GetBookingAsync(Expression<Func<Booking, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<ServiceResponse<BookingDTO?>> CreateBookingAsync(BookingCreateDTO bookingCreateDTO);
        Task<ServiceResponse<BookingDTO?>> UpdateBookingAsync(int id, BookingUpdateDTO bookingUpdateDTO);
        Task<ServiceResponse<BookingDTO?>> RemoveBookingAsync(int id);
    }
}
