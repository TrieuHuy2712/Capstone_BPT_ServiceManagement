using System;
using System.Threading.Tasks;
using BPT_Service.Application.TagService.ViewModel;

namespace BPT_Service.Application.TagService.Query.GetByIDTagServiceAsync
{
    public interface IGetByIDTagServiceAsyncQuery
    {
          Task<TagViewModel> ExecuteAsync(Guid id);
    }
}