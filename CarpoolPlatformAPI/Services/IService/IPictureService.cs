using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.Ride;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IPictureService
    {
        Task<ServiceResponse<PictureDTO?>> UploadPictureAsync(PictureCreateDTO pictureCreateDTO);
        Task<ServiceResponse<PictureDTO?>> RemovePictureAsync(int id);
    }
}
