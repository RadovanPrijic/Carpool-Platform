using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Ride
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string StartLocation { get; set; }

        [Required]
        public string EndLocation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public double PricePerSeat { get; set; }

        [MaxLength(500)]
        public string? RideDescription { get; set; }

        [Required]
        [MaxLength(250)]
        public string CarInfo { get; set; }

        public int SeatsAvailable { get; set; } = 3;

        public bool TwoInBackseat { get; set; } = true;

        [Required]
        public string LuggageSize { get; set; }

        public bool InsuranceStatus { get; set; } = true;

        public bool AutomaticBooking { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
