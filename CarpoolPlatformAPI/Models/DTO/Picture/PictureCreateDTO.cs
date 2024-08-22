using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Picture
{
    public class PictureCreateDTO
    {
        [Required]
        public IFormFile File { get; set; }
    }
}
