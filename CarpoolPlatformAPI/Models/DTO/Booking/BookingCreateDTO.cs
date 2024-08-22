using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Booking
{
    public class BookingCreateDTO
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public int RideId { get; set; }
    }
}
