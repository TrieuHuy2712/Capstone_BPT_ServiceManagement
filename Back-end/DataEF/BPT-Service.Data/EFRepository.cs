using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BPT_Service.Data.Infrastructure.SharedKernel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BPT_Service.Data
{
    public class EFRepository<T, K> : IRepository<T, K>, IDisposable where T : DomainEntity<K>
    {
        private readonly AppDbContext _context;

        public EFRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task Add(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        public async Task<IEnumerable<T>> FindAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            return await this.FindAll((Expression<Func<T, object>>[])includeProperties).ToListAsync();
        }
        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return await this.FindAll(predicate, (Expression<Func<T, object>>[])includeProperties).ToListAsync();
        }

        public async Task<T> FindByIdAsync(K id, params Expression<Func<T, object>>[] includeProperties)
        {
            return await this.FindAll((Expression<Func<T, object>>[])includeProperties).SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public async Task<T> FindSingleAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return await this.FindAll((Expression<Func<T, object>>[])includeProperties).SingleOrDefaultAsync(predicate);
        }

        public async Task Add(List<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
        }
        public void Remove(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void Remove(K id)
        {
            var entity = FindById(id);
            Remove(entity);
        }

        public void RemoveMultiple(List<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        #region 
        private IQueryable<T> FindAll(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items;
        }

        private IQueryable<T> FindAll(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> items = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    items = items.Include(includeProperty);
                }
            }
            return items.Where(predicate);
        }
        private T FindById(K id, params Expression<Func<T, object>>[] includeProperties)
        {
            return this.FindAll((Expression<Func<T, object>>[])includeProperties).SingleOrDefault(x => x.Id.Equals(id));
        }

        private T FindSingle(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
        {
            return this.FindAll((Expression<Func<T, object>>[])includeProperties).SingleOrDefault(predicate);
        }


        #endregion
    }
}