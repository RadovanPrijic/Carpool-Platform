using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.User
{
    public class PasswordDTO
    {
        [Required(ErrorMessage = "You have not entered your current password.")]
        [StringLength(30, ErrorMessage = "The password should be between 8 and 30 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your new password.")]
        [StringLength(30, ErrorMessage = "The password should be between 8 and 30 characters long.", MinimumLength = 8)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered the confirmation of your new password.")]
        [Compare("NewPassword", ErrorMessage = "The entered passwords do not match.")]
        public string NewPasswordConfirmation { get; set; } = null!;
    }
}
