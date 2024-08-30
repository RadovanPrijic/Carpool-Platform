using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarpoolPlatformAPI.Models.DTO.Auth
{
    public class RegistrationRequestDTO
    {
        [Required(ErrorMessage = "You have not entered an email address.")]
        [EmailAddress(ErrorMessage = "The entered email address is not writen in a valid email format.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered a password.")]
        [StringLength(30, ErrorMessage = "The password should be between 8 and 30 characters long.", MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your first name.")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your last name.")]
        public string LastName { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your phone number.")]
        [Phone(ErrorMessage = "The entered phone number is not writen in a valid phone number format.")]
        public string PhoneNumber { get; set; } = null!;

        [Required(ErrorMessage = "You have not entered your birth date.")]
        [DataType(DataType.Date, ErrorMessage = "The entered birth date is not a Date type.")]
        public DateTime BirthDate { get; set; }

        [StringLength(500, ErrorMessage = "The profile biography should be up to 500 characters long.")]
        public string? ProfileBio { get; set; }

        public string? ChattinessPrefs { get; set; }
        public string? MusicPrefs { get; set; }
        public string? SmokingPrefs { get; set; }
        public string? PetsPrefs { get; set; }
    }
}
