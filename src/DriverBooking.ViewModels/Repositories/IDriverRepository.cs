using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models;
using DriverBooking.Core.SeedWorks;

namespace DriverBooking.Core.Repositories
{
    public interface IDriverRepository : IRepository<Driver,int>
    {
        public Task<PageResult<Driver>> GetFreeDriverAsync(int pageIndex, int pageSize);
    }
}
