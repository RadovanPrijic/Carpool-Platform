using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.IValidation;
using System.Linq.Expressions;
using System.Net;

namespace CarpoolPlatformAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRideRepository _rideRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IValidationService _validationService;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IUserRepository userRepository, IRideRepository rideRepository,
            IBookingRepository bookingRepository, INotificationRepository notificationRepository, IValidationService validationService, 
                IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _userRepository = userRepository;
            _rideRepository = rideRepository;
            _bookingRepository = bookingRepository;
            _notificationRepository = notificationRepository;
            _validationService = validationService;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<ReviewDTO>>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null)
        {
            var reviews = await _reviewRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return new ServiceResponse<List<ReviewDTO>>(HttpStatusCode.OK, _mapper.Map<List<ReviewDTO>>(reviews));
        }

        public async Task<ServiceResponse<ReviewDTO?>> GetReviewAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null)
        {
            var review = await _reviewRepository.GetAsync(filter, includeProperties, notTracked);

            if (review == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The review has not been found.");
            }

            return new ServiceResponse<ReviewDTO?>(HttpStatusCode.OK, _mapper.Map<ReviewDTO>(review));
        }

        public async Task<ServiceResponse<ReviewDTO?>> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            var review = _mapper.Map<Review>(reviewCreateDTO);
            review.CreatedAt = DateTime.Now;
            var reviewer = await _userRepository.GetAsync(u => u.Id == reviewCreateDTO.ReviewerId && u.DeletedAt == null);
            var reviewee = await _userRepository.GetAsync(u => u.Id == reviewCreateDTO.RevieweeId && u.DeletedAt == null,
                includeProperties: "ReceivedReviews");
            var ride = await _rideRepository.GetAsync(r => r.Id == reviewCreateDTO.RideId && r.DeletedAt == null,
                includeProperties: "Reviews");
            var booking = await _bookingRepository.GetAsync(b => b.Id == reviewCreateDTO.BookingId && b.DeletedAt == null ,
                includeProperties: "User");

            //if (_validationService.GetCurrentUserId() != reviewCreateDTO.ReviewerId)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.Forbidden, "You are not allowed to post this review.");
            //}
            /*else*/ if (reviewer  == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The reviewer has not been found.");
            }
            else if (reviewee == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The reviewee has not been found.");
            }
            else if (ride == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The ride has not been found.");
            }
            else if (booking == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The booking has not been found.");
            }
            //else if (reviewer.Id == reviewee.Id)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.BadRequest, "You can not review your own ride.");
            //}
            //else if(ride.Reviews.FirstOrDefault(r => r.ReviewerId == reviewer.Id && r.DeletedAt == null) != null)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.BadRequest, "You have already reviewed this ride.");
            //}
            //else if (ride.UserId != reviewee.Id || ride.Bookings.FirstOrDefault(b => b.UserId == reviewer.Id) == null)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.BadRequest,
            //        "The provided ride is not associated with the provided reviewee and/or the provided booking. ");
            //}
            //else if (booking.User.Id != reviewer.Id || booking.BookingStatus != "accepted")
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.BadRequest,
            //        "You cannot review a ride for which you have no accepted booking.");
            //}
            //else if (ride.DepartureTime > DateTime.Now)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.BadRequest, "Only your past rides can be reviewed.");
            //}

            booking.Review = review;
            booking.UpdatedAt = DateTime.Now;

            ride.Reviews.Add(review);
            ride.UpdatedAt = DateTime.Now;

            reviewer.GivenReviews.Add(review);
            reviewer.UpdatedAt = DateTime.Now;

            int numberOfReviews = reviewee.ReceivedReviews.Count;
            reviewee.Rating = (numberOfReviews * reviewee.Rating + review.Rating) / (numberOfReviews + 1);
            reviewee.ReceivedReviews.Add(review);

            review = await _reviewRepository.CreateAsync(review);

            var notification = new Notification
            {
                Message = $"Your ride has been reviewed by {reviewer.FirstName} {reviewer.LastName}.",
                UserId = reviewee.Id,
                CreatedAt = DateTime.Now
            };
            reviewee.Notifications.Add(notification);
            reviewee.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<ReviewDTO?>(HttpStatusCode.OK, _mapper.Map<ReviewDTO>(review));
        }

        public async Task<ServiceResponse<ReviewDTO?>> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO)
        {
            var review = await _reviewRepository.GetAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "Reviewer, Reviewee, Reviewee.ReceivedReviews, Ride");

            if (review == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The review has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != review.ReviewerId)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.Forbidden, "You are not allowed to edit this review.");
            //}

            var reviewer = review.Reviewer;
            var reviewee = review.Reviewee;

            if (reviewUpdateDTO.Rating != review.Rating)
            {
                int numberOfReviews = reviewee.ReceivedReviews.Count;
                reviewee.Rating = ((numberOfReviews * reviewee.Rating) + (reviewUpdateDTO.Rating - review.Rating)) / numberOfReviews;
            }
            _mapper.Map(reviewUpdateDTO, review);
            review.UpdatedAt = DateTime.Now;
            review = await _reviewRepository.UpdateAsync(review);

            var notification = new Notification
            {
                Message = $"The review for your ride by {reviewer.FirstName} {reviewer.LastName} has just been updated.",
                UserId = reviewee.Id,
                CreatedAt = DateTime.Now
            };
            reviewee.Notifications.Add(notification);
            reviewee.UpdatedAt = DateTime.Now;
            await _notificationRepository.CreateAsync(notification);

            return new ServiceResponse<ReviewDTO?>(HttpStatusCode.OK, _mapper.Map<ReviewDTO>(review));
        }

        public async Task<ServiceResponse<ReviewDTO?>> RemoveReviewAsync(int id)
        {
            var review = await _reviewRepository.GetAsync(
                r => r.Id == id &&
                     r.DeletedAt == null,
                     includeProperties: "Reviewer, Reviewee, Reviewee.ReceivedReviews, Ride, Booking");

            if (review == null)
            {
                return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NotFound, "The review has not been found.");
            }
            //else if(_validationService.GetCurrentUserId() != review.ReviewerId)
            //{
            //    return new ServiceResponse<ReviewDTO?>(HttpStatusCode.Forbidden, "You are not allowed to edit this review.");
            //}

            var reviewee = review.Reviewee;         
            int ratingSum = reviewee.ReceivedReviews.Sum(r => r.Rating);
            int numberOfReviews = reviewee.ReceivedReviews.Count;
            if (numberOfReviews > 0)
            {
                reviewee.Rating = ratingSum / numberOfReviews;
            }
            else
            {
                reviewee.Rating = 0;
            }
            reviewee.UpdatedAt = DateTime.Now;

            review.DeletedAt = DateTime.Now;
            await _reviewRepository.UpdateAsync(review);

            return new ServiceResponse<ReviewDTO?>(HttpStatusCode.NoContent);
        }
    }
}
