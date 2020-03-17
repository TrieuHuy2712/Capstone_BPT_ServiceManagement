using BPT_Service.Application.FollowingPostService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Query.GetFollowByUser
{
    public interface IGetFollowByUserQuery
    {
        Task<List<ServiceFollowingUserViewModel>> ExecuteAsync(string userId);
    }
}
