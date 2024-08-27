using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Review;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IReviewService
    {
        Task<ServiceResponse<List<ReviewDTO>>> GetAllReviewsAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<ServiceResponse<ReviewDTO?>> GetReviewAsync(Expression<Func<Review, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<ServiceResponse<ReviewDTO?>> CreateReviewAsync(ReviewCreateDTO reviewCreateDTO);
        Task<ServiceResponse<ReviewDTO?>> UpdateReviewAsync(int id, ReviewUpdateDTO reviewUpdateDTO);
        Task<ServiceResponse<ReviewDTO?>> RemoveReviewAsync(int id);
    }
}
