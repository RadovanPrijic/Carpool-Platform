using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class ReviewService : IReviewService
    {
        public Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO)
        {
            throw new NotImplementedException();
        }

        public Task<List<ReviewDTO>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null, int pageSize = 0, int pageNumber = 1)
        {
            throw new NotImplementedException();
        }

        public Task<ReviewDTO?> GetReviewAsync(Expression<Func<Review, bool>> filter = null, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<ReviewDTO?> RemoveReviewAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ReviewDTO?> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO)
        {
            throw new NotImplementedException();
        }
    }
}
