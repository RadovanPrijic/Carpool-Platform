using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Booking
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string BookingStatus { get; set; } = null!;
        public int SeatsBooked { get; set; }
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDTO User { get; set; } = null!;
        public RideDTO Ride { get; set; } = null!;
        public ReviewDTO? Review { get; set; }
    }
}
