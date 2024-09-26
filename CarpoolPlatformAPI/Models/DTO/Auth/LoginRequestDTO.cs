using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Login
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "You have not entered an email address.")]
        [EmailAddress(ErrorMessage = "The entered email address is not writen in a valid email format.")]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "You have not entered a password.")]
        public string Password { get; set; } = null!;
    }
}
