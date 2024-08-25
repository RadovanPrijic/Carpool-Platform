using CarpoolPlatformAPI.Models.DTO.Picture;
using System.Security.Claims;

namespace CarpoolPlatformAPI.Util
{
    public class ValidationService : IValidationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ValidationService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetCurrentUserEmail()
        {
            var email = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            return email;
        }

        public string? GetCurrentUserId()
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId;
        }

        public bool ValidateFileUpload(PictureCreateDTO pictureCreateDTO)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(pictureCreateDTO.File.FileName)) || pictureCreateDTO.File.Length > 10485760)
            {
                return false;
            }

            return true;
        }
    }
}
