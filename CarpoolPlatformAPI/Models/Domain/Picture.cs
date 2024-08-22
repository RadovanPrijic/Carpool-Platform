using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.Domain
{
    public class Picture
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string FilePath { get; set; }
        public string FileExtension { get; set; }
        public string FileName { get; set; }
        public long FileSizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public string UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
