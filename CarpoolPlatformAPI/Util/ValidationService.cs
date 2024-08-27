using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Util.IValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;
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

        public static IActionResult HandleServiceResponse<T>(
            ServiceResponse<T> serviceResponse,
            ControllerBase? controller = null,
            string? actionName = null,
            object? routeValues = null)
        {
            if (serviceResponse.StatusCode == HttpStatusCode.Created && controller != null && actionName != null && routeValues != null)
            {
                return controller.CreatedAtAction(actionName, routeValues, serviceResponse.Data);
            }

            return serviceResponse.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(serviceResponse.Data),
                HttpStatusCode.NotFound => new NotFoundObjectResult(new { message = serviceResponse.ErrorMessage }),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(new { message = serviceResponse.ErrorMessage }),
                HttpStatusCode.Forbidden => new ObjectResult(new { message = serviceResponse.ErrorMessage }) { StatusCode = StatusCodes.Status403Forbidden },
                HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(new { message = serviceResponse.ErrorMessage }),
                HttpStatusCode.NoContent => new NoContentResult(),
                _ => new ObjectResult("An unexpected error occurred.") { StatusCode = StatusCodes.Status500InternalServerError }
            };
        }

    }
}
