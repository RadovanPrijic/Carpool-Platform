using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Picture
{
    public class PictureCreateDTO
    {
        [Required(ErrorMessage = "You have not uploaded a profile picture.")]
        public IFormFile File { get; set; }

        [Required(ErrorMessage = "You have not provided a user ID in your profile picture upload.")]
        public string UserId { get; set; }
    }
}
