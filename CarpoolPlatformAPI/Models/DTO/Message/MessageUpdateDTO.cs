using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Message
{
    public class MessageUpdateDTO
    {
        public string? Content { get; set; }

        public bool ReadStatus { get; set; } = true;
    }
}
