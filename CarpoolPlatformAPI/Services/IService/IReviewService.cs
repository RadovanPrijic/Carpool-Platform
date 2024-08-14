using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Review;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IReviewService
    {
        Task<List<ReviewDTO>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1);
        Task<ReviewDTO?> GetReviewAsync(Expression<Func<Review, bool>> filter = null, string? includeProperties = null);
        Task<ReviewDTO> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        Task<ReviewDTO?> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO);
        Task<ReviewDTO?> RemoveReviewAsync(int id);
    }
}
