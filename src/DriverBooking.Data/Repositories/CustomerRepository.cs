using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Domain.Identity;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data.Repositories
{
    public class CustomerRepository : Repository<Customer, int>, ICustomerRepository
    {
        public CustomerRepository(DriverBookingContext context) : base(context)
        {
        }

        public Task<Customer?> GetCustomerByAccountId(Guid customerAccId)
        {
            var customer = _context.Customers.SingleOrDefaultAsync(c => c.CustomerAccountId == customerAccId);
            return customer;
        }

    }
}
