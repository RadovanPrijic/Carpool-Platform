using AutoMapper;
using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly CarpoolPlatformDbContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserController(CarpoolPlatformDbContext dbContext, IUserRepository userRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _userRepository.Login(loginRequestDTO);

            if (string.IsNullOrEmpty(loginResponse.Token))
            {
                return BadRequest(new { message = "You have entered an incorrect email address or password." });
            }

            return Ok(loginResponse);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            bool isUserUnique = await _userRepository.isUserUnique(registrationRequestDTO.Email);

            if (!isUserUnique)
            {
                return BadRequest(new { message = "The entered email address already exists." });
            }

            var userDomainModel = await _userRepository.Register(registrationRequestDTO);

            if (userDomainModel == null)
            {
                return BadRequest(new { message = "An error has occured in the registration process." });
            }

            return Ok(userDomainModel);
        }
    }
}
