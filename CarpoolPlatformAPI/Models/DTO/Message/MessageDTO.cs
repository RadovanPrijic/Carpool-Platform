using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public bool ReadStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDTO Sender { get; set; } = null!;
        public UserDTO Receiver { get; set; } = null!;
    }
}
