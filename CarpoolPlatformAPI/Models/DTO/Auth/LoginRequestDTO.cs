using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Login
{
    public class LoginRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
