using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.Core.Repositories
{
    public interface ITripRepository : IRepository<Trip, Guid>
    {
        /// <summary>
        /// Gets all trips for a specific driver.
        /// </summary>
        /// <param name="driverId">The ID of the driver.</param>
        /// <returns>A collection of trips associated with the specified driver.</returns>
        Task<IEnumerable<Trip>> GetTripsByDriverIdAsync(int driverId);
        /// <summary>
        /// Gets all trips for a specific passenger.
        /// </summary>
        /// <param name="passengerId">The ID of the passenger.</param>
        /// <returns>A collection of trips associated with the specified passenger.</returns>
        Task<IEnumerable<Trip>> GetTripsByPassengerIdAsync(int passengerId);
    }
}
