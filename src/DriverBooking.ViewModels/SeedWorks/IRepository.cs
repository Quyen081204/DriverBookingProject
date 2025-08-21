using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DriverBooking.Core.SeedWorks
{
    public interface IRepository<T, PK> where T : class
        // PK is the type of the primary key for the entity T
        // T is the type of the entity being managed by the repository
        // This interface defines a generic repository pattern for CRUD operations on entities of type T
    {
        Task<T> GetByIdAsync(PK id);
        Task<IEnumerable<T>> GetAllAsync();
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

    }
}
