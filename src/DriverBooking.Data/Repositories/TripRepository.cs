using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data.Repositories
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

        public async Task UpdateTrip(Guid id, Trip trip)
        {
            if (id != trip.Id)
                throw new ArgumentException("Id mismatch");

            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
        }

        public async Task ReloadTripAsync(Trip trip)
        {
            await _context.Entry(trip).ReloadAsync();
        }

        public Task<Trip?> GetTripById(Guid id)
        {
            return _context.Trips.SingleOrDefaultAsync(t => t.Id == id);
        }
    }
}
