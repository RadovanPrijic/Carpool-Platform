using CarpoolPlatformAPI.Models.DTO.Picture;

namespace CarpoolPlatformAPI.Util.IValidation
{
    public interface IValidationService
    {
        public string? GetCurrentUserEmail();
        public string? GetCurrentUserId();
        public bool ValidateFileUpload(PictureCreateDTO pictureCreateDTO);
    }
}
