using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.SeedWorks;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Repositories
{
    public interface IDriverRepository : IRepository<Driver,int>
    {
        Task<IEnumerable<AvailableDriverLocation>> GetDriversWithinMetersAsync(CustomerRequirements customerRequirements);
        Task<IEnumerable<AvailableDriverLocation>> GetDriversWithinMetersNoVehicleTypeAsync(CustomerRequirements customerRequirements);

        Task<Driver?> GetDriverByAccountId(Guid driverAccId);

        Task<string?> GetDriverUserNameById(int Id);

        Task<Driver?> GetDriverById(int id);
    }
}
