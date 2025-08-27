using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.SeedWorks;
using NetTopologySuite.Geometries;

namespace DriverBooking.Core.Repositories
{
    public interface IDriverRepository : IRepository<Driver,int>
    {
        public Task<IEnumerable<AvailableDriverLocation>> GetLocationFreeDriversWithinMetersAsync(double customer_lat, double customer_lon,float withinM);
    }
}
