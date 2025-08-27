using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Home : ControllerBase
    {
        [HttpGet("Home")]
        [Authorize(Roles ="Admin,Customer")]
        public IActionResult Index()
        {
            return Ok("Welcome to Driver Booking API");
        }
    }
}
