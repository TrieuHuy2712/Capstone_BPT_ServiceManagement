using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BPT_Service.Model.Infrastructure.Interfaces
{
    public interface IRepository<T, K> where T : class
    {
        Task<T> FindByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties);

        Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> FindAllAsync(params Expression<Func<T, object>>[] includeProperties);

        Task<T> FindSingleDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties);

        Task Add(T entity);

        Task Add(List<T> entities);

        void Update(T entity);

        void Remove(T entity);

        void Remove(K id);

        void RemoveMultiple(List<T> entities);

        Task SaveAsync();
    }
}