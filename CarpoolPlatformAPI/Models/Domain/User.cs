using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class User : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [MaxLength(500)]
        public string? ProfileBio { get; set; }

        [Range(1, 5)]
        public double? Rating { get; set; } = 0;

        public string? ChattinessPrefs { get; set; }

        public string? MusicPrefs { get; set; }

        public string? SmokingPrefs { get; set; }

        public string? PetsPrefs { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public Picture? Picture { get; set; }

        public ICollection<Ride> Rides { get; } = new List<Ride>();

        public ICollection<Booking> Bookings { get; } = new List<Booking>();

        public ICollection<Review> GivenReviews { get; } = new List<Review>();

        public ICollection<Review> ReceivedReviews { get; } = new List<Review>();

        public ICollection<Message> SentMessages { get; } = new List<Message>();

        public ICollection<Message> ReceivedMessages { get; } = new List<Message>();

        public ICollection<Notification> Notifications { get; } = new List<Notification>();
    }
}
