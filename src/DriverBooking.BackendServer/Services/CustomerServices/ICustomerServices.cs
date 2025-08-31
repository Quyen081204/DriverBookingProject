using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Customer;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.API.Services.CustomerServices
{
    public interface ICustomerServices
    {
        // Create customer and customer account , role with it
        Task<ApiResponse<AuthenticatedResult>> RegisterCustomer(CustomerDTO customer);
    }
}
