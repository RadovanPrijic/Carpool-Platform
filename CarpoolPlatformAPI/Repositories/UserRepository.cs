using AutoMapper;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CarpoolPlatformAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CarpoolPlatformDbContext _db;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private string _secretKey;

        public UserRepository(CarpoolPlatformDbContext db, UserManager<User> userManager, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _userManager = userManager;
            _mapper = mapper;
            _secretKey = configuration.GetValue<string>("Jwt:SecretKey");
        }

        public async Task<bool> isUserUnique(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

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
                            new Claim(ClaimTypes.Name, user.Id),
                            new Claim(ClaimTypes.Email, user.Email),
                            new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddHours(6),
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
                UserName = registrationRequestDTO.Email
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Basic_User");
                    var userToReturn = await _db.Users.FirstOrDefaultAsync(u => u.Email == registrationRequestDTO.Email);

                    return _mapper.Map<UserDTO>(user);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return new UserDTO();
        }
    }
}
