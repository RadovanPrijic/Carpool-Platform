namespace CarpoolPlatformAPI.Models.DTO.Notification
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public bool CheckedStatus { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
