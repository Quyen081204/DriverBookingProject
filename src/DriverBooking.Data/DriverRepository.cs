using AutoMapper;
using DriverBooking.Core.Domain.Entities;
using DriverBooking.Core.Models;
using DriverBooking.Core.Repositories;
using DriverBooking.Data.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data
{
    public class DriverRepository : Repository<Driver, int>, IDriverRepository
    {
        private readonly IMapper _mapper;
        public DriverRepository(DriverBookingContext context, IMapper mapper) : base(context)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<PageResult<Driver>> GetFreeDriverAsync(int pageIndex, int pageSize)
        {
            var query = _context.Drivers.AsQueryable();

            query = query.Where(d => d.DriverStatus == DriverStatus.ON)
                         .OrderBy(d => d.Id)
                         .Skip((pageIndex -1) * pageSize)
                         .Take(pageSize);

            var totalCount = await query.CountAsync();

            return new PageResult<Driver>
            {
                Results = await query.ToListAsync(),
                RowCount = totalCount,
                CurrentPage = pageIndex,
                PageSize = pageSize
            };
        }
    }
}
