using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.Ride;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IPictureService
    {
        Task<PictureDTO?> GetPictureAsync(Expression<Func<Picture, bool>>? filter = null);
        Task<PictureDTO> CreatePictureAsync(PictureCreateDTO pictureCreateDTO);
        Task<PictureDTO?> RemovePictureAsync(int id);
    }
}
