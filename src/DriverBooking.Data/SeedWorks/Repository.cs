using System.Linq.Expressions;
using DriverBooking.Core.SeedWorks;
using Microsoft.EntityFrameworkCore;

namespace DriverBooking.Data.SeedWorks
{
    public class Repository<T, PK> : IRepository<T, PK> where T : class
    {
        protected readonly DriverBookingContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(DriverBookingContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            // use translator to DECIDE HOW we want the query to be executed
            return _dbSet.Where(expression);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(PK id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
// Understand unit of work pattern and how it relates to repository pattern. -> It is used for mangging transactions and coordinating changes across multiple repositories it tracked entity affected by repo and apply the changes and resolution of concurrency problems
// Make asyn version of repository pattern.
// Resgister in DI