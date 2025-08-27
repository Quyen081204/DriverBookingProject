using AutoMapper;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models.Booking;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace DriverBooking.Data
{
    public class DriverRepository : Repository<Driver, int>, IDriverRepository
    {
        private readonly IMapper _mapper;
        public DriverRepository(DriverBookingContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<IEnumerable<AvailableDriverLocation>> GetLocationFreeDriversWithinMetersAsync(double lat, double lon,float withinM)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var customerLocation = geometryFactory.CreatePoint(new Coordinate(lon, lat));

            var freeDrivers = await _context.Drivers
                .Where(d => d.DriverAccount.IsActive
                            && d.DriverStatus == DriverStatus.ON
                            && d.CurrentLocation != null
                            && EF.Functions.IsWithinDistance(d.CurrentLocation, customerLocation, withinM, true))
                .Select(d => new AvailableDriverLocation
                {
                    DriverId = d.Id,
                    CurrentLocation = new Core.Models.Common.PointDTO
                    {
                        Lat = d.CurrentLocation.Coordinate.Y,
                        Lon = d.CurrentLocation.Coordinate.X
                    },
                    Distance = EF.Functions.Distance(customerLocation, d.CurrentLocation, true)
                }).ToListAsync();

            if (freeDrivers.Count > 10)
            {
                freeDrivers = freeDrivers.Take(10).ToList();
            }

            return freeDrivers;
        }
    }
}
