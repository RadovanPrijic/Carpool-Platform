using AutoMapper;
using CarpoolPlatformAPI.CustomActionFilters;
using CarpoolPlatformAPI.Data;
using CarpoolPlatformAPI.Models.DTO.Auth;
using CarpoolPlatformAPI.Models.DTO.Login;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

namespace CarpoolPlatformAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPictureService _pictureService;

        public UsersController(IUserService userService, IPictureService pictureService)
        {
            _userService = userService;
            _pictureService = pictureService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var serviceResponse = await _userService.Login(loginRequestDTO);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        [ValidateModel]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO registrationRequestDTO)
        {
            var serviceResponse = await _userService.Register(registrationRequestDTO);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var serviceResponse = await _userService.GetUserAsync(
                u => u.Id == id &&
                     u.DeletedAt == null,
                     includeProperties: "Picture, Notifications");

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            var serviceResponse = await _userService.UpdateUserAsync(id, userUpdateDTO);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpGet]
        [Route("notifications/{id}")]
        public async Task<IActionResult> GetAllNotificationsForUser([FromRoute] string userId)
        {
            var serviceResponse = await _userService.GetAllNotificationsForUser(userId);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpPost]
        [Route("upload-profile-picture")]
        [ValidateModel]
        public async Task<IActionResult> UploadProfilePicture([FromForm] PictureCreateDTO pictureCreateDTO)
        {
            var serviceResponse = await _pictureService.UploadPictureAsync(pictureCreateDTO);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }

        [HttpDelete]
        [Route("remove-profile-picture/{id:int}")]
        public async Task<IActionResult> DeleteProfilePicture([FromRoute] int id)
        {
            var serviceResponse = await _pictureService.RemovePictureAsync(id);

            return ValidationService.HandleServiceResponse(serviceResponse);
        }
    }
}
