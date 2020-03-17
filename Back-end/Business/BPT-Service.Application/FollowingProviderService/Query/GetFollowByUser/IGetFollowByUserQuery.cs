using BPT_Service.Application.FollowingProviderService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Query.GetFollowByUser
{
    public interface IGetFollowByUserQuery
    {
        Task<List<ProviderFollowingByUserViewModel>> ExecuteAsync(string userId);
    }
}
