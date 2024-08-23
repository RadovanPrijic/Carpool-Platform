using CarpoolPlatformAPI.Models.DTO.Booking;
using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Ride
{
    public class RideDTO
    {
        public int Id { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public double PricePerSeat { get; set; }
        public string? RideDescription { get; set; }
        public string CarInfo { get; set; }
        public int SeatsAvailable { get; set; }
        public bool TwoInBackseat { get; set; }
        public string LuggageSize { get; set; }
        public bool InsuranceStatus { get; set; }
        public bool AutomaticBooking { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDTO User { get; set; } = null!;
        public ICollection<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
    }
}
