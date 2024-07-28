using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Auth
{
    public class RegistrationRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
