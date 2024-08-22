using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Ride
{
    public class RideCreateDTO
    {
        [Required]
        public double PricePerSeat { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
