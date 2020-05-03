using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Query.GetFollowByProvider
{
    public class GetFollowByProviderQuery : IGetFollowByProviderQuery
    {
        private readonly UserManager<AppUser> _userRepository;
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        public GetFollowByProviderQuery(UserManager<AppUser> userRepository,
            IRepository<ProviderFollowing, int> providerFollowingRepository)
        {
            _userRepository = userRepository;
            _providerFollowingRepository = providerFollowingRepository;
        }

        public async Task<List<UserFollowingByProviderViewModel>> ExecuteAsync(string providerId)
        {
            var getAllProviderFollowing = await _providerFollowingRepository.FindAllAsync(x=>x.ProviderId == Guid.Parse(providerId));
            var getAllUser = await _userRepository.Users.ToListAsync();
            var query = (from follow in getAllProviderFollowing.ToList()
                         join user in getAllUser.ToList()
                         on follow.UserId equals user.Id
                         select new UserFollowingByProviderViewModel
                         {
                              UserId = follow.UserId.ToString(),
                              DateCreated = follow.DateCreated,
                              IsReceiveEmail = follow.IsReceiveEmail,
                              UserName = user.UserName
                         }).ToList();
            return query;
        }
    }
}
