using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.User
{
    public class UserUpdateDTO
    {
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
