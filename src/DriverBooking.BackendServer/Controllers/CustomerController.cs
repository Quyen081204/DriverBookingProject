using DriverBooking.API.Services.CustomerServices;
using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DriverBooking.API.Controllers
{
    // Api handle information about Customer , history trip, change profile ....
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerServices _customerServices;

        public CustomerController(ICustomerServices customerServices)
        {
            _customerServices = customerServices;
        }
        /// <summary>
        /// Api for customer register account and create profile to get avartar url first upload file by calling API upload
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthenticatedResult>>> RegisterCustomer(CustomerDTO customer)
        {
            if (!ModelState.IsValid)
                return BadRequest("Error customer binding");

            return await _customerServices.RegisterCustomer(customer);
        }
    }
}
