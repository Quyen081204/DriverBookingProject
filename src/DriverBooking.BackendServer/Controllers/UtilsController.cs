using DriverBooking.API.Services.UploadServices;
using DriverBooking.Core.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UtilsController : ControllerBase
    {
        // test upload file
        private readonly IUploadService _uploadService;

        public UtilsController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost("upload")] public async Task<ActionResult<UploadResultDTO>> Upload([FromForm]UploadFileRequestDTO uploadRequest)
        {
            var result = await _uploadService.UploadFile(uploadRequest.File, uploadRequest.FolderName);
            return Ok(result);
        }
    }
}
