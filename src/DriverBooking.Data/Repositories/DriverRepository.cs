using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Models.Common;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace DriverBooking.Data.Repositories
{
    public class DriverRepository : Repository<Driver, int>, IDriverRepository
    {
        public DriverRepository(DriverBookingContext context) : base(context)
        {
        }

        public Task<Driver?> GetDriverByAccountId(Guid driverAccId)
        {
            var driver = _context.Drivers.SingleOrDefaultAsync(d => d.DriverAccountId == driverAccId);
            return driver;
        }

        public async Task<IEnumerable<AvailableDriverLocation>> GetDriversWithinMetersAsync(CustomerRequirements customerRequirements)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var customerLocation = geometryFactory.CreatePoint(new Coordinate(customerRequirements.lon, customerRequirements.lat));

            var freeDrivers = await _context.Drivers
                .Where(d => d.DriverAccount.IsActive
                            && d.DriverStatus == DriverStatus.ON
                            && d.CurrentLocation != null
                            && d.Vehicle.VehicleCapacity == customerRequirements.vehicleCapacity
                            && d.Vehicle.VehicleType == customerRequirements.VehicleType
                            && EF.Functions.IsWithinDistance(d.CurrentLocation, customerLocation, customerRequirements.withinM, true))
                .Select(d => new AvailableDriverLocation
                {
                    DriverId = d.Id,
                    DriverUserName = d.DriverAccount.UserName,
                    CurrentLocation = new Core.Models.Common.PointDTO
                    {
                        Lat = d.CurrentLocation.Coordinate.Y,
                        Lon = d.CurrentLocation.Coordinate.X
                    },
                    Distance = EF.Functions.Distance(customerLocation, d.CurrentLocation, true)
                }).ToListAsync();

            if (freeDrivers.Count > 10)
            {
                freeDrivers = freeDrivers.Take(10).OrderBy(d => d.Distance).ToList();
            }

            return freeDrivers;
        }

        public async Task<IEnumerable<AvailableDriverLocation>> GetDriversWithinMetersNoVehicleTypeAsync(CustomerRequirements customerRequirements)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var customerLocation = geometryFactory.CreatePoint(new Coordinate(customerRequirements.lon, customerRequirements.lat));

            var freeDrivers = await _context.Drivers
                .Where(d => d.DriverAccount.IsActive
                            && d.DriverStatus == DriverStatus.ON
                            && d.CurrentLocation != null
                            && d.Vehicle.VehicleCapacity == customerRequirements.vehicleCapacity
                            && EF.Functions.IsWithinDistance(d.CurrentLocation, customerLocation, customerRequirements.withinM, true))
                .Select(d => new AvailableDriverLocation
                {
                    DriverId = d.Id,
                    DriverUserName = d.DriverAccount.UserName,
                    CurrentLocation = new Core.Models.Common.PointDTO
                    {
                        Lat = d.CurrentLocation.Coordinate.Y,
                        Lon = d.CurrentLocation.Coordinate.X
                    },
                    Distance = EF.Functions.Distance(customerLocation, d.CurrentLocation, true)
                }).ToListAsync();

            if (freeDrivers.Count > 10)
            {
                freeDrivers = freeDrivers.Take(10).OrderBy(d => d.Distance).ToList();
            }

            return freeDrivers;
        }

        public async Task<string?> GetDriverUserNameById(int Id)
        {
            var driver = await _context.Drivers.SingleOrDefaultAsync(d => d.Id == Id);
            var userName = driver?.DriverAccount.UserName;
            return userName;
        }

        public Task<Driver?> GetDriverById(int id) 
        {
            var driver = _context.Drivers.SingleOrDefaultAsync(d => d.Id == id);
            return driver;
        }
    }
}
