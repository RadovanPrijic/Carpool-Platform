using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Message
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public bool ReadStatus { get; set; } = false;

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [Required]
        public string SenderId { get; set; }

        [Required]
        public User Sender { get; set; } = null!;

        [Required]
        public string ReceiverId { get; set; }

        [Required]
        public User Receiver { get; set; } = null!;
    }
}
