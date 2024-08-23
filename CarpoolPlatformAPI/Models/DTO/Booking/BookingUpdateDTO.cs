using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Booking
{
    public class BookingUpdateDTO
    {
        [Required(ErrorMessage = "You have not provided a booking status with your booking.")]
        public string BookingStatus { get; set; }
    }
}
