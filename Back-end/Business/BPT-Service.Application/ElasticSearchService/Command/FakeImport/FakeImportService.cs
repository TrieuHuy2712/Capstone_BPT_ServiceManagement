using Bogus;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.IRepositories;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.FakeImport
{
    public class FakeImportService : IFakeImportService
    {
        private readonly IElasticSearchRepository<PostServiceViewModel> _elasticSearchRepository;

        public FakeImportService(IElasticSearchRepository<PostServiceViewModel> elasticSearchRepository)
        {
            _elasticSearchRepository = elasticSearchRepository;
        }

        public async Task<Faker<PostServiceViewModel>> ExecuteAsync(int count)
        {
            try
            {
                var productFaker = new Faker<PostServiceViewModel>()
                   .CustomInstantiator(f => new PostServiceViewModel())
                   .RuleFor(p => p.Author, f => f.Name.LastName())
                   .RuleFor(p => p.AvtService, f => f.Image.LoremFlickrUrl())
                   .RuleFor(p => p.CategoryId, f => f.Random.Number(1000))
                   .RuleFor(p => p.CategoryName, f => f.Commerce.ProductMaterial())
                   .RuleFor(p => p.DateCreated, f => f.Date.Recent())
                   .RuleFor(p => p.Description, f => f.Lorem.Text())
                   .RuleFor(p => p.Email, f => f.Person.Email)
                   .RuleFor(p => p.Rating, f => f.Random.Float(0, 1))
                   .RuleFor(p => p.IsProvider, f => f.Random.Bool())
                   .RuleFor(p => p.ServiceName, f => f.Commerce.Product());

                var products = productFaker.Generate(count);
                await _elasticSearchRepository.SaveManyAsync(products.ToArray());
                return productFaker;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}