using Bogus;
using BPT_Service.Application.PostService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.ElasticSearchService.Command.FakeImport
{
    public interface IFakeImportService
    {
        Task<Faker<PostServiceViewModel>> ExecuteAsync(int count);
    }
}