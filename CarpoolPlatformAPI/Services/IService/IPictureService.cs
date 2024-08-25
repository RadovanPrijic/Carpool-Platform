using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IPictureService
    {
        Task<PictureDTO?> UploadPictureAsync(PictureCreateDTO pictureCreateDTO);
        Task<PictureDTO?> RemovePictureAsync(int id);
    }
}
