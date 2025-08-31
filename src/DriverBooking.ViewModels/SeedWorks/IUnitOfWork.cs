using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Repositories;

namespace DriverBooking.Core.SeedWorks
{
    public interface IUnitOfWork
    {
        IRepository<OpeningFee, int> _openingFeeRepository { get; }
        IRepository<StageFee, int> _stageFeeRepository { get; }
        IDriverRepository _driverRepository { get; }  

        ICustomerRepository _customerRepository { get; }
        Task<int> CompleteAsync();
    }
}
