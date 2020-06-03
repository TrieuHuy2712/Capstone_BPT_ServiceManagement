using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Model.IRepositories
{
    public interface IElasticSearchRepository<T> where T : class
    {
        //Task<IEnumerable<T>> GetProducts(int count, int skip = 0);

        //Task<T> GetServiceById(int id);

        //Task<IEnumerable<T>> GetServicesByCategory(string category);

        //Task<IEnumerable<T>> GetServicesByBrand(string category);

        Task DeleteAsync(T entity);
        Task UpdateAsync(T entity);

        Task SaveSingleAsync(T entity);

        Task SaveManyAsync(T[] entities);

        Task SaveBulkAsync(T[] entities);
    }
}