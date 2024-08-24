using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.Notification;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;

namespace CarpoolPlatformAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;
        private string _secretKey;

        public UserService(IUserRepository userRepository, INotificationRepository notificationRepository,  UserManager<User> userManager,
            IMapper mapper, IConfiguration configuration, IValidationService validationService)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _userManager = userManager;
            _mapper = mapper;
            _validationService = validationService;
            _secretKey = configuration.GetValue<string>("Jwt:SecretKey")!;
        }

        public async Task<bool> isUserUnique(string email)
        {
            var user = await _userRepository.GetAsync(u => u.Email == email);

            if (user == null)
            {
                return true;
            }

            return false;
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
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
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddHours(4),
                        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    LoginResponseDTO loginResponseDTO = new LoginResponseDTO()
                    {
                        Token = tokenHandler.WriteToken(token)
                    };

                    return loginResponseDTO;
                }
            }

            return new LoginResponseDTO()
            {
                Token = ""
            };
        }

        public async Task<UserDTO> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            User user = new()
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

                    var userToReturn = await _userRepository.GetAsync(
                        u => u.Email == registrationRequestDTO.Email,
                        includeProperties: "Picture, Notifications");

                    return _mapper.Map<UserDTO>(userToReturn);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return new UserDTO();
        }

        public async Task<List<UserDTO>> GetAllUsersAsync(Expression<Func<User, bool>>? filter, string? includeProperties,
            int pageSize, int pageNumber, bool? notTracked = null)
        {
            var users = await _userRepository.GetAllAsync(filter, includeProperties, pageSize, pageNumber, notTracked);

            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<UserDTO?> GetUserAsync(Expression<Func<User, bool>>? filter, string? includeProperties,
            bool? notTracked = null)
        {
            var user = await _userRepository.GetAsync(filter, includeProperties, notTracked);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> UpdateUserAsync(string id, UserUpdateDTO userUpdateDTO)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return null;
            }

            user = _mapper.Map<User>(userUpdateDTO);
            user.UpdatedAt = DateTime.Now;
            user = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<UserDTO?> RemoveUserAsync(string id)
        {
            var user = await _userRepository.GetAsync(u => u.Id == id && u.DeletedAt == null);

            if (user == null)
            {
                return null;
            }

            user.DeletedAt = DateTime.Now;

            //TODO Update associated entities

            user = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<NotificationDTO>> GetAllNotificationsForUser(Expression<Func<Notification, bool>> filter)
        {
            var notifications = await _notificationRepository.GetAllAsync(filter);

            return _mapper.Map<List<NotificationDTO>>(notifications.OrderBy(n => n.CreatedAt));
        }
    }
}
