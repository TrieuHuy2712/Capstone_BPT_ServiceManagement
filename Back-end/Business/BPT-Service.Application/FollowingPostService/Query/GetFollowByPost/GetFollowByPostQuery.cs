using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Query.GetFollowByPost
{
    public class GetFollowByPostQuery : IGetFollowByPostQuery
    {
        private readonly IRepository<AppUser, Guid> _userRepository;
        private readonly IRepository<ServiceFollowing, int> _serviceFollwingRepository;
        public GetFollowByPostQuery(IRepository<AppUser, Guid> userRepository,
            IRepository<ServiceFollowing, int> serviceFollwingRepository)
        {
            _userRepository = userRepository;
            _serviceFollwingRepository = serviceFollwingRepository;
        }

        public async Task<List<ServiceFollowingPostViewModel>> ExecuteAsync(string idService)
        {
            var getAllFollowing = await _serviceFollwingRepository.FindAllAsync(x => x.ServiceId == Guid.Parse(idService));
            var getAllUser = await _userRepository.FindAllAsync();
            var data = (from follow in getAllFollowing.ToList()
                        join user in getAllUser.ToList()
                        on follow.UserId equals user.Id
                        select new ServiceFollowingPostViewModel
                        {
                            Id = follow.Id,
                            ServiceId = follow.ServiceId.ToString(),
                            DateCreated = follow.DateCreated,
                            UserId = follow.UserId.ToString(),
                            UserName = user.UserName
                        }).ToList();
            return data;
        }
    }
}
