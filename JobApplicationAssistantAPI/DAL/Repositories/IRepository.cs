using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? include = null);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate,
                               Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                               bool trackChanges = true);

        Task<T> GetByIDAsync(int id);
        Task InsertAsync(T obj);
        Task DeleteAsync(int id);
        Task UpdateAsync(T obj);
    }
}
