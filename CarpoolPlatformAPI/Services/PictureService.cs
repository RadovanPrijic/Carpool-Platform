using AutoMapper;
using CarpoolPlatformAPI.Models.Domain;
using CarpoolPlatformAPI.Models.DTO.Picture;
using CarpoolPlatformAPI.Repositories.IRepository;
using CarpoolPlatformAPI.Services.IService;
using System.Linq.Expressions;

namespace CarpoolPlatformAPI.Services
{
    public class PictureService : IPictureService
    {
        private readonly IPictureRepository _pictureRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PictureService(IPictureRepository pictureRepository, IMapper mapper, 
            IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _pictureRepository = pictureRepository;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<PictureDTO?> GetPictureAsync(Expression<Func<Picture, bool>>? filter = null)
        {
            throw new NotImplementedException();
        }

        public async Task<PictureDTO> CreatePictureAsync(PictureCreateDTO pictureCreateDTO)
        {
            ValidateFileUpload(pictureCreateDTO);

            Picture picture = _mapper.Map<Picture>(pictureCreateDTO);
            picture.FileName = Guid.NewGuid().ToString();
            
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Pictures",
                $"{picture.FileName}{picture.FileExtension}");

            using var stream = new FileStream(localFilePath, FileMode.Create);
            await picture.File.CopyToAsync(stream);

            var urlFilePath = $"{_httpContextAccessor.HttpContext!.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}{_httpContextAccessor.HttpContext.Request.PathBase}/Pictures/{picture.FileName}{picture.FileExtension}";
            picture.FilePath = urlFilePath;

            picture = await _pictureRepository.CreateAsync(picture);

            return _mapper.Map<PictureDTO>(picture);
        }

        public async Task<PictureDTO?> RemovePictureAsync(int id)
        {
            throw new NotImplementedException();
        }

        private void ValidateFileUpload(PictureCreateDTO pictureCreateDTO)
        {
            var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

            if (!allowedExtensions.Contains(Path.GetExtension(pictureCreateDTO.File.FileName)))
            {
                // Throw Error("Unsupported file extension")
            }

            if (pictureCreateDTO.File.Length > 10485760)
            {
                // Throw Error("File size more than 10MB, please upload a smaller size file.");
            }
        }
    }
}
