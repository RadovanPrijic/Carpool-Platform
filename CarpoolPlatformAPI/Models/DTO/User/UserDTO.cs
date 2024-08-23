using CarpoolPlatformAPI.Models.DTO.Notification;
using CarpoolPlatformAPI.Models.DTO.Picture;

namespace CarpoolPlatformAPI.Models.DTO.User
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public string? ProfileBio { get; set; }
        public double? Rating { get; set; } = 0;
        public string? ChattinessPrefs { get; set; }
        public string? MusicPrefs { get; set; }
        public string? SmokingPrefs { get; set; }
        public string? PetsPrefs { get; set; }
        public string ExperienceLevel { get; set; }
        public string? ReliabilityLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public PictureDTO? Picture { get; set; }
        public ICollection<NotificationDTO>? Notifications { get; } = new List<NotificationDTO>();
    }
}
