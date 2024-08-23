using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Booking
{
    public class BookingCreateDTO
    {
        [Required(ErrorMessage = "You have not chosen the number of seats to be booked.")]
        public int SeatsBooked { get; set; } = 0;

        [Required(ErrorMessage = "You have not provided a price total with your booking.")]
        public double TotalPrice { get; set; }

        [Required(ErrorMessage = "You have not provided a user ID with your booking.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "You have not provided a ride ID with your booking.")]
        public int RideId { get; set; }
    }
}
