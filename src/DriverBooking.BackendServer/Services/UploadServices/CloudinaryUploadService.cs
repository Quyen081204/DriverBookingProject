
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DriverBooking.Core.ConfigOptions;
using DriverBooking.Core.Models.Common;
using Microsoft.Extensions.Options;

namespace DriverBooking.API.Services.UploadServices
{
    public class CloudinaryUploadService : IUploadService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryUploadService> _logger;
        public CloudinaryUploadService(IOptions<CloudinarySettings> config, ILogger<CloudinaryUploadService> logger)
        {
            var account = new Account(
            config.Value.CloudName,
            config.Value.ApiKey,
            config.Value.ApiSecret
        );
            _cloudinary = new Cloudinary(account);

            _logger = logger;
        }
        public async Task<UploadResultDTO>UploadFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
                _logger.LogError("Can not upload file null or empty to cloudinary");

            // Upload to Cloudinary
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = folderName // optional: put files in a folder
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new UploadResultDTO
                {
                    UrlFile = uploadResult.SecureUrl.ToString(),
                    Success = true
                };
            }

            _logger.LogError(uploadResult.Error.Message);

            return new UploadResultDTO
            {
                UrlFile = "can not upload file",
                Success = false
            };
        }

        
    }
}
