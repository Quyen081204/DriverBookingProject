using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DriverBookingContext _context;

        public UnitOfWork(DriverBookingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }   
    }
}
