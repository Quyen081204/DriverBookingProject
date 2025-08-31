using Microsoft.AspNetCore.Http;

namespace DriverBooking.Core.Models.Common
{
    public class UploadFileRequestDTO
    {
        public IFormFile File { get; set; }

        public string FolderName { get; set; }
    }
}
