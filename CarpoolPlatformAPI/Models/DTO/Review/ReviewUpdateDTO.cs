using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Review
{
    public class ReviewUpdateDTO
    {
        [Required(ErrorMessage = "You have not chosen a rating.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "You have not entered a comment.")]
        [StringLength(1000, ErrorMessage = "The review comment should be up to 1000 characters long.")]
        public string Comment { get; set; }
    }
}
