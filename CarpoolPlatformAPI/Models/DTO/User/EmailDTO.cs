using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.User
{
    public class EmailDTO
    {
        [Required(ErrorMessage = "You have not entered your current email address.")]
        [EmailAddress(ErrorMessage = "The entered email address is not writen in a valid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your new email address.")]
        [EmailAddress(ErrorMessage = "The entered email address is not writen in a valid email format.")]
        public string NewEmail { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered the confirmation of your new email address.")]
        [Compare("NewEmail",  ErrorMessage = "The entered email addresses do not match.")]
        public string NewEmailConfirmation { get; set; } = null!;
    }
}
