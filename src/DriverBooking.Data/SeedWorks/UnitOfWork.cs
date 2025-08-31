using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Repositories;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.Data.SeedWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DriverBookingContext _context;

        public IRepository<OpeningFee, int> _openingFeeRepository { get; }
        public IRepository<StageFee, int> _stageFeeRepository { get; }
        public IDriverRepository _driverRepository { get; }
        public ICustomerRepository _customerRepository { get; }

        public UnitOfWork(DriverBookingContext context, 
                          IRepository<OpeningFee, int> openingFeeRepository,
                          IRepository<StageFee, int> stageFeeRepository,
                          IDriverRepository driverRepository,
                          ICustomerRepository customerRepository)
        {
            _context = context;
            _openingFeeRepository = openingFeeRepository;
            _stageFeeRepository = stageFeeRepository;
            _driverRepository = driverRepository;
            _customerRepository = customerRepository;
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
