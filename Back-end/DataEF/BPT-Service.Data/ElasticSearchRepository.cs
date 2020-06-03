using BPT_Service.Model.IRepositories;
using Nest;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Data
{
    public class ElasticSearchRepository<T> : IElasticSearchRepository<T> where T : class
    {
        private readonly IElasticClient _elasticClient;

        public ElasticSearchRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task DeleteAsync(T entity)
        {
            await _elasticClient.DeleteAsync<T>(entity);
        }

        //public virtual Task<IEnumerable<T>> GetServices(int count, int skip = 0)
        //{
        //    var products = cli
        //}

        //public Task<T> GetServiceById(int id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<T>> GetServicesByBrand(string category)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<T>> GetServicesByCategory(string category)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task UpdateAsync(T entity)
        {
            await _elasticClient.UpdateAsync<T>(entity, u => u.Doc(entity));
        }

        public async Task SaveBulkAsync(T[] entities)
        {
            var result = await _elasticClient.BulkAsync(b => b.Index("service_management").IndexMany(entities));
            if (result.Errors)
            {
                // the response can be inspected for errors
                foreach (var item in result.ItemsWithErrors)
                {
                    Console.WriteLine("Failed to index document {0} : {1}", item.Id, item.Error);
                }
            }
        }

        public async Task SaveManyAsync(T[] entities)
        {
            var result = await _elasticClient.IndexManyAsync(entities);
            if (result.Errors)
            {
                foreach (var item in result.ItemsWithErrors)
                {
                    Console.WriteLine("Failed to index document {0} : {1}", item.Id, item.Error);
                }
            }
        }

        public async Task SaveSingleAsync(T entity)
        {
            await _elasticClient.IndexDocumentAsync<T>(entity);
        }
    }
}