using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Review
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [Required]
        public string ReviewerId { get; set; }

        [Required]
        public User Reviewer { get; set; } = null!;

        [Required]
        public string RevieweeId { get; set; }

        [Required]
        public User Reviewee { get; set; } = null!;

        [Required]
        public int RideId { get; set; }

        [Required]
        public Ride Ride { get; set; } = null!;

        [Required]
        public int BookingId { get; set; }

        [Required]
        public Booking Booking { get; set; } = null!;
    }
}
