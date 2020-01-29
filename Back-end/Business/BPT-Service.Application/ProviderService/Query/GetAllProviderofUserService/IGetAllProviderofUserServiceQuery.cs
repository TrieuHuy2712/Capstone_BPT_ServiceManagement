using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;

namespace BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService
{
    public interface IGetAllProviderofUserServiceQuery
    {
         Task<List<ProviderServiceViewModel>> ExecuteAsync();
    }
}