using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.Notification;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.Email;
using CarpoolPlatformAPI.Util.IValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CarpoolPlatformAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IValidationService _validationService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private string _secretKey;
        private string _emailConfirmationEndpoint;
        private string _emailChangeEndpoint;
        private string _passwordResetEndpoint;

        public UserService(IUserRepository userRepository, INotificationRepository notificationRepository, IValidationService validationService,
            IEmailService emailService, IMapper mapper, UserManager<User> userManager, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _validationService = validationService;
            _emailService = emailService;
            _mapper = mapper;
            _userManager = userManager;
            _secretKey = configuration.GetValue<string>("Jwt:SecretKey")!;
            _emailConfirmationEndpoint = configuration.GetValue<string>("Endpoints:EmailConfirmationEndpoint")!;
            _emailChangeEndpoint = configuration.GetValue<string>("Endpoints:EmailChangeEndpoint")!;
            _passwordResetEndpoint = configuration.GetValue<string>("Endpoints:PasswordResetEndpoint")!;
        }

        public async Task<ServiceResponse<LoginResponseDTO?>> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Email == loginRequestDTO.Email && u.DeletedAt == null);

            if (user != null)
            {
                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

                if (isValid)
                {
                    //var roles = (List<string>)await _userManager.GetRolesAsync(user);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(
                        [
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Email, user.Email!),
                            //new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
                        ]),
                        Expires = DateTime.UtcNow.AddHours(4),
                        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    LoginResponseDTO loginResponseDTO = new()
                    {
                        Token = tokenHandler.WriteToken(token),
                        EmailConfirmed = user.EmailConfirmed
                    };

                    return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.OK, loginResponseDTO);
                }

                return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.Unauthorized, "You have entered an incorrect password.");
            }

            return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.Unauthorized, "You have entered an incorrect email address.");
        }

        public async Task<ServiceResponse<UserDTO?>> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Email == registrationRequestDTO.Email && u.DeletedAt == null);

            if (user != null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.BadRequest, "The entered email address already exists in the database.");
            }

            user = _mapper.Map<User>(registrationRequestDTO);
            user.NormalizedEmail = registrationRequestDTO.Email.ToUpper();
            user.UserName = registrationRequestDTO.Email;
            user.EmailConfirmed = false;
            user.CreatedAt = DateTime.Now;

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);

                if (result.Succeeded)
                {
                    //await _userManager.AddToRoleAsync(user, "Basic_User");
                    var userToReturn = await _userRepository.GetAsync(u => u.Email == registrationRequestDTO.Email);

                    var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(userToReturn!);
                    var subject = "Email confirmation";
                    var body = $"<p>Click <a href='{_emailConfirmationEndpoint}?userId={user.Id}&confirmationToken={confirmationToken}'>" +
                        $"here</a> to confirm your email address.</p>";
                    await _emailService.SendEmailAsync(user.Email!, subject, body);

                    var notification = new Notification
                    {
                        Message = "You have been successfully registered. Complete your profile by confirming your email address and uploading a profile picture.",
                        UserId = userToReturn!.Id,
                        CreatedAt = DateTime.Now
                    };
                    userToReturn!.Notifications.Add(notification);
                    await _notificationRepository.CreateAsync(notification);

                    return new ServiceResponse<UserDTO?>(HttpStatusCode.OK, _mapper.Map<UserDTO>(userToReturn));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return new ServiceResponse<UserDTO?>(HttpStatusCode.InternalServerError, "An unexpected error has occured.");
        }

        public async Task<ServiceResponse<List<UserDTO>>> GetAllUsersAsync(Expression<Func<User, bool>>? filter, string? includeProperties,
            int pageSize, int pageNumber, bool? notTracked = null)
        {
            var users = await _userRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return new ServiceResponse<List<UserDTO>>(HttpStatusCode.OK, _mapper.Map<List<UserDTO>>(users));
        }

        public async Task<ServiceResponse<UserDTO?>> GetUserAsync(Expression<Func<User, bool>>? filter, string? includeProperties,
            bool? notTracked = null)
        {
            var user = await _userRepository.GetAsync(filter, includeProperties, notTracked);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }

            return new ServiceResponse<UserDTO?>(HttpStatusCode.OK, _mapper.Map<UserDTO>(user));
        }

        public async Task<ServiceResponse<UserDTO?>> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != user.Id)
            //{
            //    return new ServiceResponse<UserDTO?>(HttpStatusCode.Forbidden, "You are not allowed to update this user.");
            //}

            _mapper.Map(userUpdateDTO, user);
            user.UpdatedAt = DateTime.Now;
            user = await _userRepository.UpdateAsync(user);

            return new ServiceResponse<UserDTO?>(HttpStatusCode.OK, _mapper.Map<UserDTO>(user));
        }

        public async Task<ServiceResponse<List<NotificationDTO>>> GetAllNotificationsForUser(string id)
        {
            //if (_validationService.GetCurrentUserId() != id)
            //{
            //    return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.Forbidden, "You are not allowed to access this information.");
            //}

            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == id && n.DeletedAt == null);

            return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.OK, 
                _mapper.Map<List<NotificationDTO>>(notifications.OrderBy(n => n.CreatedAt)));
        }

        public async Task<ServiceResponse<List<NotificationDTO>>> MarkUserNotificationsAsChecked(string id)
        {
            //if (_validationService.GetCurrentUserId() != id)
            //{
            //    return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.Forbidden, "You are not allowed to access this information.");
            //}

            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == id && n.DeletedAt == null);
            foreach (var notification in notifications)
            {
                notification.CheckedStatus = true;
            }
            await _notificationRepository.SaveAsync();

            return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.OK,
                _mapper.Map<List<NotificationDTO>>(notifications.OrderBy(n => n.CreatedAt)));
        }

        public async Task<ServiceResponse<UserDTO?>> InitiateEmailConfirmationAsync(string id)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != id)
            //{
            //    return new ServiceResponse<UserDTO?>(HttpStatusCode.Forbidden, "You are not allowed to do this action.");
            //}
            else if (user.EmailConfirmed)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.BadRequest, "Your email address has already been confirmed.");
            }

            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var subject = "Email confirmation";
            var body = $"<p>Click <a href='{_emailConfirmationEndpoint}?userId={user.Id}&confirmationToken={confirmationToken}'>" +
                $"here</a> to confirm your email address.</p>";
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return new ServiceResponse<UserDTO?>(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResponse<UserDTO?>> InitiateEmailChangeAsync(string id, EmailDTO emailDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != id)
            //{
            //    return new ServiceResponse<UserDTO?>(HttpStatusCode.Forbidden, "You are not allowed to do this action.");
            //}

            var changeToken = await _userManager.GenerateChangeEmailTokenAsync(user, emailDTO.NewEmail);
            var subject = "Email change";
            var body = 
                $"<p>Click <a href='{_emailChangeEndpoint}?userId={user.Id}&changeToken={changeToken}&newEmail={emailDTO.NewEmail}'>" +
                $"here</a> to change your email address.</p>";
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return new ServiceResponse<UserDTO?>(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResponse<UserDTO?>> ConfirmEmailAsync(string id, string token, bool emailChange,
            string? newEmail)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != id)
            //{
            //    return new ServiceResponse<UserDTO?>(HttpStatusCode.Forbidden, "You are not allowed to do this action.");
            //}
            else if (!emailChange && user.EmailConfirmed)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.BadRequest, "Your email address has already been confirmed.");
            }

            IdentityResult result = emailChange 
                ? await _userManager.ChangeEmailAsync(user, newEmail!, token)
                : await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NoContent, 
                    $"Your email address has been successfully {(emailChange ? "changed" : "confirmed")}.");
            }

            return new ServiceResponse<UserDTO?>(HttpStatusCode.InternalServerError, "An unexpected error has occurred.");
        }

        public async Task<ServiceResponse<UserDTO?>>InitiatePasswordResetAsync(string email)
        {
            var user = await _userRepository.GetAsync(u => u.Email == email && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }

            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var subject = "Password reset";
            var body =
                $"<p>Click <a href='{_passwordResetEndpoint}?userEmail={email}&resetToken={resetToken}'> here</a> " +
                $"to reset your password.</p>";
            await _emailService.SendEmailAsync(user.Email!, subject, body);

            return new ServiceResponse<UserDTO?>(HttpStatusCode.NoContent);
        }

        public async Task<ServiceResponse<UserDTO?>> ResetPasswordAsync(string email, string resetToken, PasswordDTO passwordDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Email == email && u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }

            IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, passwordDTO.NewPassword);

            if (result.Succeeded)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NoContent);
            }

            return new ServiceResponse<UserDTO?>(HttpStatusCode.InternalServerError, "An unexpected error has occurred.");
        }
    }
}
