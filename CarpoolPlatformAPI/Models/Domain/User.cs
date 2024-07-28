using Microsoft.AspNetCore.Identity;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateOnly BirthDate { get; set; }
        public string? ProfileBio { get; set; }
        public double? Rating { get; set; }
        public string? ChattinessPrefs { get; set; }
        public string? MusicPrefs { get; set; }
        public string? SmokingPrefs { get; set; }
        public string? PetsPrefs { get; set; }
        public string ExperienceLevel { get; set; }
        public string ReliabilityLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

    }
}
