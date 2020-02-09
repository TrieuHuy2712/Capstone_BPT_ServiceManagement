using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;

namespace BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService
{
    public interface IGetByIdProviderNewsServiceQuery
    {
          Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int id);
    }
}