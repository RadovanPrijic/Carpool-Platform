using AutoMapper;
using Azure.Core;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Models.DTO.User;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using CarpoolPlatformAPI.Util;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

        public PictureService(IPictureRepository pictureRepository, IUserRepository userRepository, IMapper mapper, 
            IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor, IValidationService validationService)
        {
            _pictureRepository = pictureRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _validationService = validationService;
        }

        public async Task<PictureDTO?> UploadPictureAsync(PictureCreateDTO pictureCreateDTO)
        {
            var user = await _userRepository.GetAsync(
                u => u.Id == pictureCreateDTO.UserId && 
                u.DeletedAt == null, 
                includeProperties: "Picture");

            if(!_validationService.ValidateFileUpload(pictureCreateDTO))
            {
                return null;
            }

            var picture = new Picture
            {
                File = pictureCreateDTO.File,
                FileExtension = Path.GetExtension(pictureCreateDTO.File.FileName),
                FileSizeInBytes = pictureCreateDTO.File.Length,
                FileName = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.Now,
                UserId = pictureCreateDTO.UserId
            };

            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Pictures",
                $"{picture.FileName}{picture.FileExtension}");

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await picture.File.CopyToAsync(stream);

            var urlFilePath = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Pictures/{picture.FileName}{picture.FileExtension}";
            picture.FilePath = urlFilePath;

            if (user!.Picture != null)
            {
                user.Picture.File = picture.File;
                user.Picture.FilePath = picture.FilePath;
                user.Picture.FileExtension = picture.FileExtension;
                user.Picture.FileName = picture.FileName;
                user.Picture.FileSizeInBytes = picture.FileSizeInBytes;
                user.Picture.UpdatedAt = DateTime.Now;
                user.UpdatedAt = DateTime.Now;
                await _userRepository.SaveAsync();
            }
            else
            {
                user.Picture = picture;
                user.UpdatedAt = DateTime.Now;
                picture = await _pictureRepository.CreateAsync(picture);
            }

            return _mapper.Map<PictureDTO>(picture);
        }

        public async Task<PictureDTO?> RemovePictureAsync(int id)
        {
            var picture = await _pictureRepository.GetAsync(
                p => p.Id == id && 
                p.DeletedAt == null,
                includeProperties: "User, User.Picture");

            if (picture == null || (picture.User.Id != _validationService.GetCurrentUserId()))
            {
                return null;
            }

            picture.DeletedAt = DateTime.Now;

            var user = picture.User;
            user.Picture = null;
            user.UpdatedAt = DateTime.Now;

            picture = await _pictureRepository.UpdateAsync(picture);

            return _mapper.Map<PictureDTO>(picture);
        }
    }
}
