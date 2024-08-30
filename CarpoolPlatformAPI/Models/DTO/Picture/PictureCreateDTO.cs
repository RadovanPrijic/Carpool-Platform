using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Picture
{
    public class PictureCreateDTO
    {
        [Required(ErrorMessage = "You have not provided a profile picture.")]
        public IFormFile File { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a user ID.")]
        public string UserId { get; set; } = null!;
    }
}
