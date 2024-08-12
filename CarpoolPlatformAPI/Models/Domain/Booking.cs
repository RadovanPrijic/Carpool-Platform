using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Booking
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string BookingStatus { get; set; }
        public int SeatsBooked { get; set; } = 0;
        public double TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; } = null!;
        public int RideId { get; set; }
        public Ride Ride { get; set; } = null!;
        public Review? Review { get; set; }
    }
}
