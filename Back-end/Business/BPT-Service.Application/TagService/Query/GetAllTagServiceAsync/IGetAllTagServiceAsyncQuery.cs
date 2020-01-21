using System.Collections.Generic;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;

namespace BPT_Service.Application.TagService.Query.GetAllServiceAsync
{
    public interface IGetAllTagServiceAsyncQuery
    {
         Task<List<TagViewModel>> ExecuteAsync();
    }
}