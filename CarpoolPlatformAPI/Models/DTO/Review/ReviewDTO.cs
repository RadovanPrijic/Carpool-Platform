using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Review
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserDTO Reviewer { get; set; } = null!;
        public string RevieweeId { get; set; }
        public RideDTO Ride { get; set; } = null!;
    }
}
