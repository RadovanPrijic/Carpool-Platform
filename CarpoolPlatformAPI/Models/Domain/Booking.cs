using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Booking
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string BookingStatus { get; set; }

        [Required]
        public int SeatsBooked { get; set; } = 0;

        [Required]
        public double TotalPrice { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public int RideId { get; set; }

        public Ride Ride { get; set; } = null!;

        public Review? Review { get; set; }
    }
}
