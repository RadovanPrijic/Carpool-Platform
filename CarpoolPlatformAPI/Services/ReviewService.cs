using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            var review = await _reviewRepository.CreateAsync(_mapper.Map<Review>(reviewCreateDTO));

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<List<ReviewDTO>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            var reviews = await _reviewRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber);

            return _mapper.Map<List<ReviewDTO>>(reviews);
        }

        public async Task<ReviewDTO?> GetReviewAsync(Expression<Func<Review, bool>> filter = null, string? includeProperties = null)
        {
            var review = await _reviewRepository.GetAsync(filter, includeProperties);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> RemoveReviewAsync(int id)
        {
            var review = await _reviewRepository.GetAsync(r => r.Id == id);

            if (review == null)
            {
                return null;
            }

            review.DeletedAt = DateTime.Now;
            review = await _reviewRepository.UpdateAsync(review);

            return _mapper.Map<ReviewDTO>(review);
        }

        public async Task<ReviewDTO?> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO)
        {
            var review = await _reviewRepository.GetAsync(r => r.Id == id);

            if (review == null)
            {
                return null;
            }

            review = await _reviewRepository.UpdateAsync(_mapper.Map<Review>(reviewUpdateDTO));

            return _mapper.Map<ReviewDTO>(review);
        }
    }
}
