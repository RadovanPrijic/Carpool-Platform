﻿using AutoMapper;
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
        private readonly IValidationService _validationService;

        public UsersController(IUserService userService, IPictureService pictureService, IValidationService validationService)
        {
            _userService = userService;
            _pictureService = pictureService;
            _validationService = validationService;
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        [ValidateModel]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
        {
            var loginResponse = await _userService.Login(loginRequestDTO);

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
            bool isUserUnique = await _userService.IsUserUnique(registrationRequestDTO.Email);

            if (!isUserUnique)
            {
                return BadRequest(new { message = "The entered email address already exists." });
            }

            var userDTO = await _userService.Register(registrationRequestDTO);

            if (userDTO == null)
            {
                return BadRequest(new { message = "You have entered invalid registration data." });
            }

            return Ok(userDTO);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute] string id)
        {
            var userDTO = await _userService.GetUserAsync(
                u => u.Id == id &&
                u.DeletedAt == null,
                includeProperties: "Picture, Notifications");

            if (userDTO == null)
            {
                return NotFound(new { message = "The user has not been found." });
            }

            return Ok(userDTO);
        }

        [HttpPut]
        [Route("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser([FromRoute] string id, [FromBody] UserUpdateDTO userUpdateDTO)
        {
            if (_validationService.GetCurrentUserId() != id)
            {
                return Unauthorized(new { message = "You are not authorized to update this user."});
            }

            var userDTO = await _userService.UpdateUserAsync(id, userUpdateDTO);

            if (userDTO == null)
            {
                return NotFound(new { message = "The user has not been found." });
            }

            return Ok(userDTO);
        }

        [HttpGet]
        [Route("notifications/{id}")]
        public async Task<IActionResult> GetAllNotificationsForUser([FromRoute] string userId)
        {
            if (_validationService.GetCurrentUserId() != userId)
            {
                return Unauthorized(new { message = "You are not authorized to access this information." });
            }

            var notificationDTOs = await _userService.GetAllNotificationsForUser(n => n.UserId == userId);

            return Ok(notificationDTOs);
        }

        [HttpPost]
        [Route("upload-profile-picture")]
        [ValidateModel]
        public async Task<IActionResult> UploadProfilePicture([FromForm] PictureCreateDTO pictureCreateDTO)
        {
            if (_validationService.GetCurrentUserId() != pictureCreateDTO.UserId)
            {
                return Unauthorized(new { message = "You are not authorized to upload this profile picture." });
            }

            var pictureDTO = await _pictureService.UploadPictureAsync(pictureCreateDTO);

            if (pictureDTO == null)
            {
                return BadRequest(new { message = "The profile picture must be less than 10MB in size and its extension has to be one of the following: .jpg, .jpeg, .png." });
            }

            return Ok(pictureDTO);
        }

        [HttpDelete]
        [Route("remove-profile-picture/{id:int}")]
        public async Task<IActionResult> DeleteProfilePicture([FromRoute] int id)
        {
            var pictureDTO = await _pictureService.RemovePictureAsync(id);

            if (pictureDTO == null)
            {
                return NotFound(new { message = "The profile picture has not been found."});
            }

            return Ok("The profile picture has been successfully removed.");
        }

        //[HttpDelete]
        //[Route("{id}")]
        //public async Task<IActionResult> DeleteUser([FromRoute] string id)
        //{
        //    if (_validationService.GetCurrentUserId() != id)
        //    {
        //        return Unauthorized(new { message = "You are not authorized to delete this user." });
        //    }

        //    var userDTO = await _userService.RemoveUserAsync(id);

        //    if (userDTO == null)
        //    {
        //        return NotFound(new { message = "The user has not been found." });
        //    }

        //    return Ok(userDTO);
        //}
    }
}
