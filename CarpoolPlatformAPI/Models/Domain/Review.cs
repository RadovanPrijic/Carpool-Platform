using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Review
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string ReviewerId { get; set; }
        public User Reviewer { get; set; } = null!;
        public string RevieweeId { get; set; }
        public User Reviewee { get; set; } = null!;
        public int RideId { get; set; }
        public Ride Ride { get; set; } = null!;
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
    }
}
