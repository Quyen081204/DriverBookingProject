using System.Globalization;
using DriverBooking.Core.Models.Common;

namespace DriverBooking.API.Services.UploadServices
{
    public interface IUploadService
    {
        Task<UploadResultDTO>UploadFile(IFormFile file, string folderName);
    }
}
