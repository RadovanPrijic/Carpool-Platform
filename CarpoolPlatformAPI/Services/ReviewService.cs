using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Message;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IRideRepository rideRepository,
            IBookingRepository bookingRepository, INotificationRepository notificationRepository, IMapper mapper, 
                IValidationService validationService)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _rideRepository = rideRepository;
            _bookingRepository = bookingRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
            _validationService = validationService;
        }

        public async Task<List<ReviewDTO>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var reviews = await _reviewRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO?> GetReviewAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var review = await _reviewRepository.GetAsync(filter, includeProperties, notTracked);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            var review = _mapper.Map<Review>(reviewCreateDTO);
            review.CreatedAt = DateTime.Now;

            var reviewer = await _userRepository.GetAsync(u => u.Id == reviewCreateDTO.ReviewerId);
            var reviewee = await _userRepository.GetAsync(u => u.Id == reviewCreateDTO.RevieweeId);
            var ride = await _rideRepository.GetAsync(r => r.Id == reviewCreateDTO.RideId, includeProperties: "Reviews");
            var booking = await _bookingRepository.GetAsync(b => b.Id == reviewCreateDTO.BookingId, includeProperties: "User");

            if (reviewer  == null || reviewee == null || ride == null || booking == null ||
                ride.UserId != reviewee.Id ||
                ride.Bookings.FirstOrDefault(b => b.UserId == reviewer.Id) == null ||
                ride.DepartureTime > DateTime.Now ||
                booking.User.Id != reviewer.Id ||
                booking.BookingStatus != "accepted")
            {
                return null;
            }

            reviewer.GivenReviews.Add(review);
            reviewer.UpdatedAt = DateTime.Now;

            int numberOfReviews = reviewee.ReceivedReviews.Count;
            reviewee.Rating = (numberOfReviews * reviewee.Rating + reviewCreateDTO.Rating) / (numberOfReviews + 1);
            reviewee.ReceivedReviews.Add(review);

            ride.Reviews.Add(review);
            ride.UpdatedAt = DateTime.Now;

            booking.Review = review;
            booking.UpdatedAt = DateTime.Now;

            review = await _reviewRepository.CreateAsync(review);

            var notification = new Notification
            {
                Message = $"Your ride has been reviewed by {reviewer.FirstName} ${reviewer.LastName}.",
                UserId = reviewee.Id,
                CreatedAt = DateTime.Now
            };

            reviewee.Notifications.Add(notification);
            reviewee.UpdatedAt = DateTime.Now;

            await _notificationRepository.CreateAsync(notification);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO)
        {
            var review = await _reviewRepository.GetAsync(
                r => r.Id == id &&
                r.DeletedAt == null,
                includeProperties: "Reviewee, Reviewee.ReceivedReviews");

            if (review == null || _validationService.GetCurrentUserId() != review.ReviewerId)
            {
                return null;
            }

            if(reviewUpdateDTO.Rating != review.Rating)
            {
                var reviewee = review.Reviewee;
                var oldReview = reviewee.ReceivedReviews.FirstOrDefault(r => r.Id == review.Id);
                int numberOfReviews = reviewee.ReceivedReviews.Count;

                reviewee.Rating = ((numberOfReviews * reviewee.Rating) + (reviewUpdateDTO.Rating - oldReview!.Rating)) / numberOfReviews;
                reviewee.UpdatedAt = DateTime.Now;
            }

            review = _mapper.Map<Review>(reviewUpdateDTO);
            review.UpdatedAt = DateTime.Now;
            review = await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> RemoveReviewAsync(int id)
        {
            var review = await _reviewRepository.GetAsync(
                r => r.Id == id &&
                r.DeletedAt == null,
                includeProperties: "Reviewer, Reviewee, Ride, Booking");

            if (review == null || _validationService.GetCurrentUserId() != review.ReviewerId)
            {
                return null;
            }

            var reviewer = review.Reviewer;
            reviewer.GivenReviews.Remove(review);
            reviewer.UpdatedAt = DateTime.Now;

            var reviewee = review.Reviewee;
            reviewee.ReceivedReviews.Remove(review);

            // TODO Modify reviewee's rating

            reviewee.UpdatedAt = DateTime.Now;

            var ride = review.Ride;
            ride.Reviews.Remove(review);
            ride.UpdatedAt = DateTime.Now;

            var booking = review.Booking;
            booking.Review = null;
            booking.UpdatedAt = DateTime.Now;

            review.DeletedAt = DateTime.Now;
            review = await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewDTO>(review);
        }
    }
}
