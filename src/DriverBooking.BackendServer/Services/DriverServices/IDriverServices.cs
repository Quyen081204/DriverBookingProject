using DriverBooking.Core.Models.Auth;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Models.Driver;

namespace DriverBooking.API.Services.DriverServices
{
    public interface IDriverServices
    {
        Task<ApiResponse<AuthenticatedResult>> RegisterDriver(DriverDTO driverRequest);
    }
}
