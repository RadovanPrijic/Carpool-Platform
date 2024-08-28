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
        }

        public async Task<ServiceResponse<LoginResponseDTO?>> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDTO.Email);

            if (user != null)
            {
                bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

                if (isValid)
                {
                    var roles = (List<string>)await _userManager.GetRolesAsync(user);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(_secretKey);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id),
                            new Claim(ClaimTypes.Email, user.Email!),
                            new Claim(ClaimTypes.Role, roles.FirstOrDefault()!)
                        }),
                        Expires = DateTime.UtcNow.AddHours(4),
                        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                    {
                        Token = tokenHandler.WriteToken(token)
                    };

                    return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.OK, loginResponseDTO);
                }

                return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.Unauthorized, "You have entered an incorrect password.");
            }

            return new ServiceResponse<LoginResponseDTO?>(HttpStatusCode.Unauthorized, "You have entered an incorrect email address.");
        }

        public async Task<ServiceResponse<UserDTO?>> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Email == registrationRequestDTO.Email);

            if (user != null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.BadRequest, "The entered email address already exists.");
            }

            user = new()
            {
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                UserName = registrationRequestDTO.Email,
                CreatedAt = DateTime.Now
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Basic_User");
                    var userToReturn = await _userRepository.GetAsync(u => u.Email == registrationRequestDTO.Email);

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
                Console.WriteLine(e.StackTrace);
            }

            return new ServiceResponse<UserDTO?>(HttpStatusCode.InternalServerError, "An unexpected error occured.");
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
            var user = await _userRepository.GetAsync(
                u => u.Id == id &&
                u.DeletedAt == null);

            if (user == null)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            else if (_validationService.GetCurrentUserId() != user.Id)
            {
                return new ServiceResponse<UserDTO?>(HttpStatusCode.Forbidden, "You are not allowed to update this user.");
            }

            user = _mapper.Map<User>(userUpdateDTO);
            user.UpdatedAt = DateTime.Now;
            user = await _userRepository.UpdateAsync(user);

            return new ServiceResponse<UserDTO?>(HttpStatusCode.OK, _mapper.Map<UserDTO>(user));
        }

        public async Task<ServiceResponse<List<NotificationDTO>>> GetAllNotificationsForUser(string userId)
        {
            if (_validationService.GetCurrentUserId() != userId)
            {
                return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.Forbidden, "You are not allowed to access this information.");
            }

            var notifications = await _notificationRepository.GetAllAsync(n => n.UserId == userId);

            return new ServiceResponse<List<NotificationDTO>>(HttpStatusCode.OK, 
                _mapper.Map<List<NotificationDTO>>(notifications.OrderBy(n => n.CreatedAt)));
        }
    }
}
