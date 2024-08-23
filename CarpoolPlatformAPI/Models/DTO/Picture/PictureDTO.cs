using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Picture
{
    public class PictureDTO
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
    }
}
