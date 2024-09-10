using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class ConversationDTO
    {
        public UserDTO User { get; set; } = null!;
        public MessageDTO LastMessage { get; set; } = null!;
        public int UnreadMessagesCount { get; set; }
    }
}
