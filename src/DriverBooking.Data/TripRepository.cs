using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data
{
    public class TripRepository : Repository<Trip, Guid>, ITripRepository
    {
        public TripRepository(DriverBookingContext context) : base(context)
        {
        }   
        public async Task<IEnumerable<Trip>> GetTripsByDriverIdAsync(int driverId)
        {
            return await _context.Trips
                .Where(t => t.DriverId == driverId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trip>> GetTripsByPassengerIdAsync(int passengerId)
        {
            return await _context.Trips
                .Where(t => t.CustomerId == passengerId)
                .ToListAsync();
        }
    }
}
