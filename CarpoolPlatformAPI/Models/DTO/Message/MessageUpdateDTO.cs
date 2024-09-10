using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageUpdateDTO
    {
        [Required(ErrorMessage = "You have not entered a message.")]
        public string Content { get; set; } = null!;
    }
}
