using DriverBooking.API.Services.CustomerServices;
using DriverBooking.API.Services.DriverServices;
using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Driver;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly IDriverServices _driverServices;

        public DriverController(IDriverServices driverServices)
        {
            _driverServices = driverServices;
        }
        /// <summary>
        /// Api for driver register account and create profile to get avartar url first upload file by calling API upload
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthenticatedResult>>> RegisterCustomer(DriverDTO driver)
        {
            if (!ModelState.IsValid)
                return BadRequest("Error customer binding");

            return await _driverServices.RegisterDriver(driver);
        }
    }
}
