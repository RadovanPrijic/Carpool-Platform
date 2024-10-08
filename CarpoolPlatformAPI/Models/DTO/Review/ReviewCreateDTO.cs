﻿using System.ComponentModel.DataAnnotations;

namespace CarpoolPlatformAPI.Models.DTO.Review
{
    public class ReviewCreateDTO
    {
        [Required(ErrorMessage = "You have not chosen a rating.")]
        [Range(1, 5, ErrorMessage = "The rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "You have not entered a comment.")]
        [StringLength(1000, ErrorMessage = "The review comment should be up to 1000 characters long.")]
        public string Comment { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a reviewer ID with your review.")]
        public string ReviewerId { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a reviewee ID with your review.")]
        public string RevieweeId { get; set; } = null!;

        [Required(ErrorMessage = "You have not provided a ride ID with your review.")]
        public int RideId { get; set; }

        [Required(ErrorMessage = "You have not provided a booking ID with your review.")]
        public int BookingId { get; set; }
    }
}
