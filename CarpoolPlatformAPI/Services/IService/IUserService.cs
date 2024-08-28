using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.Notification;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Util;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services.IService
{
    public interface IUserService
    {
        Task<ServiceResponse<LoginResponseDTO?>> Login(LoginRequestDTO loginRequestDTO);
        Task<ServiceResponse<UserDTO?>> Register(RegistrationRequestDTO registrationRequestDTO);
        Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null,
            int pageSize = 0, int pageNumber = 1, bool? notTracked = null);
        Task<ServiceResponse<UserDTO?>> GetUserAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null,
            bool? notTracked = null);
        Task<ServiceResponse<UserDTO?>> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO);
        Task<ServiceResponse<List<NotificationDTO>>> GetAllNotificationsForUser(string id);
        Task<ServiceResponse<UserDTO?>> initiateEmailConfirmationAsync(string id);
        Task<ServiceResponse<UserDTO?>> initiateEmailChangeAsync(string id, EmailDTO emailDTO);
        Task<ServiceResponse<UserDTO?>> confirmEmailAsync(string id, string token, bool emailChange, string? newEmail);
        Task<ServiceResponse<UserDTO?>> initiatePasswordResetAsync(string email);
        Task<ServiceResponse<UserDTO?>> resetPasswordAsync(string email, string resetToken, PasswordDTO passwordDTO);
    }
}
