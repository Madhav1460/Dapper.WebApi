
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dapper.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(object id);
        Task DeleteAsync(object id);
        Task<int> AddRangeAsync(IEnumerable<T> list);
        Task UpdateAsync(T entity);
        Task AddAsync(T entity);

        //for query
        Task<T> QueryFirstOrDefaultAsync(string sql, object parameters = null);
        Task<IEnumerable<T>> QueryAsync(string sql, object parameters = null);
        Task<int> ExecuteAsync(string sql, object parameters = null);
    }
}