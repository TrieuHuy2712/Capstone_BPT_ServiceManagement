using BPT_Service.Application.FollowingPostService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Query.GetFollowByPost
{
    public interface IGetFollowByPostQuery
    {
        Task<List<ServiceFollowingPostViewModel>> ExecuteAsync(string idService);
    }
}
