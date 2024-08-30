using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageCreateDTO
    {
        [Required(ErrorMessage = "You have not entered a message.")]
        public string Content { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a sender ID with your message.")]
        public string SenderId { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a receiver ID with your message.")]
        public string ReceiverId { get; set; } = null!;
    }
}
