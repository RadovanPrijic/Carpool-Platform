using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Ride
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public DateTime DepartureTime { get; set; }
        public double PricePerSeat { get; set; }
        public string? RideDescription { get; set; }
        public string CarInfo { get; set; }
        public int SeatsAvailable { get; set; } = 3;
        public bool TwoInBackseat { get; set; } = true;
        public string LuggageSize { get; set; }
        public bool InsuranceStatus { get; set; } = true;
        public bool AutomaticBooking { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
