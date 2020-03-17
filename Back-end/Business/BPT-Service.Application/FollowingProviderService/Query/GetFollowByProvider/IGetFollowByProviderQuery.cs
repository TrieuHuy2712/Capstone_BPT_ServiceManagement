using BPT_Service.Application.FollowingProviderService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Query.GetFollowByProvider
{
    public interface IGetFollowByProviderQuery
    {
        Task<List<UserFollowingByProviderViewModel>> ExecuteAsync(string providerId);
    }
}
