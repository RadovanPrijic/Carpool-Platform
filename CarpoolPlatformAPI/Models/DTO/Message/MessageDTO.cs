using CarpoolPlatformAPI.Models.DTO.User;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool ReadStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }
}
