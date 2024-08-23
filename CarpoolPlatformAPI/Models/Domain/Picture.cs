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

        [Required]
        public string FilePath { get; set; }

        [Required]
        public string FileExtension { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public long FileSizeInBytes { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public User User { get; set; } = null!;
    }
}
