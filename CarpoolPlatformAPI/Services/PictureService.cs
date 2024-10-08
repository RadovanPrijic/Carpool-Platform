﻿using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using CarpoolPlatformAPI.Util.IValidation;
using System.Net;

namespace CarpoolPlatformAPI.Services
{
    public class PictureService : IPictureService
    {
        private readonly IPictureRepository _pictureRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidationService _validationService;

        public PictureService(IPictureRepository pictureRepository, IUserRepository userRepository, IValidationService validationService, 
            IMapper mapper, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _pictureRepository = pictureRepository;
            _userRepository = userRepository;
            _validationService = validationService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<PictureDTO?>> UploadPictureAsync(IFormFile file, string userId)
        {
            var user = await _userRepository.GetAsync(
                u => u.Id == userId && 
                     u.DeletedAt == null, 
                     includeProperties: "Picture");

            if (user == null)
            {
                return new ServiceResponse<PictureDTO?>(HttpStatusCode.NotFound, "The user has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != pictureCreateDTO.UserId)
            //{
            //    return new ServiceResponse<PictureDTO?>(HttpStatusCode.Forbidden, "You are not allowed to upload this profile picture.");
            //}
            else if (!_validationService.ValidateFileUpload(file))
            {
                return new ServiceResponse<PictureDTO?>(HttpStatusCode.BadRequest,
                    "The profile picture has to be less than 10MB in size and its file extension must be one of the following: " +
                    ".jpg, .jpeg, .png.");
            }

            var picture = user.Picture ?? new Picture
            {
                UserId = userId,
                CreatedAt = DateTime.Now
            };

            if (user.Picture != null)
            {
                picture.UpdatedAt = DateTime.Now;

                var oldPictureFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Pictures",
                    $"{user.Picture.FileName}{user.Picture.FileExtension}");

                if (File.Exists(oldPictureFilePath))
                {
                    File.Delete(oldPictureFilePath);
                }
            }       

            picture.File = file;
            picture.FileExtension = Path.GetExtension(file.FileName);
            picture.FileSizeInBytes = file.Length;
            picture.FileName = Guid.NewGuid().ToString();

            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Pictures",
                $"{picture.FileName}{picture.FileExtension}");

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await picture.File.CopyToAsync(stream);

            var urlFilePath = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}" +
                $"{_httpContextAccessor.HttpContext.Request.PathBase}/Pictures/{picture.FileName}{picture.FileExtension}";
            picture.FilePath = urlFilePath;

            user.Picture = picture;
            user.UpdatedAt = DateTime.Now;
            picture = await _pictureRepository.CreateAsync(picture);

            return new ServiceResponse<PictureDTO?>(HttpStatusCode.OK, _mapper.Map<PictureDTO>(picture));
        }

        public async Task<ServiceResponse<PictureDTO?>> RemovePictureAsync(int id)
        {
            var picture = await _pictureRepository.GetAsync(
                p => p.Id == id && 
                     p.DeletedAt == null,
                     includeProperties: "User");

            if (picture == null)
            {
                return new ServiceResponse<PictureDTO?>(HttpStatusCode.NotFound, "The profile picture has not been found.");
            }
            //else if (_validationService.GetCurrentUserId() != picture.User.Id)
            //{
            //    return new ServiceResponse<PictureDTO?>(HttpStatusCode.Forbidden, "You are not allowed to remove this profile picture.");
            //}

            var user = picture.User;
            if (user.Picture != null)
            {
                var oldPictureFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Pictures",
                    $"{user.Picture.FileName}{user.Picture.FileExtension}");
                
                if (File.Exists(oldPictureFilePath))
                {
                    File.Delete(oldPictureFilePath);
                }
            }
            user.Picture = null;
            user.UpdatedAt = DateTime.Now;
            await _pictureRepository.RemoveAsync(picture);

            return new ServiceResponse<PictureDTO?>(HttpStatusCode.NoContent);
        }
    }
}
