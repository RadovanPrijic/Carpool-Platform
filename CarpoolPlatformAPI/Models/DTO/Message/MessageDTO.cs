using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool ReadStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public UserDTO Sender { get; set; }
        public UserDTO Receiver { get; set; }
    }
}
