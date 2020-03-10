using BPT_Service.Application.FollowingProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingProviderService.Query.GetFollowByUser
{
    public class GetFollowByUserQuery : IGetFollowByUserQuery
    {
        private readonly IRepository<ProviderFollowing, int> _providerFollowingRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        public GetFollowByUserQuery(
            IRepository<ProviderFollowing, int> providerFollowingRepository,
            IRepository<Provider, Guid> providerRepository)
        {
            _providerFollowingRepository = providerFollowingRepository;
            _providerRepository = providerRepository;
        }

        public async Task<List<ProviderFollowingByUserViewModel>> ExecuteAsync(string userId)
        {
            var getAllProviderFollowing = await _providerFollowingRepository.FindAllAsync(x => x.UserId == Guid.Parse(userId));
            var getAllProvider = await _providerRepository.FindAllAsync();
            var query = (from follow in getAllProviderFollowing.ToList()
                         join provider in getAllProvider.ToList()
                         on follow.ProviderId equals provider.Id
                         select new ProviderFollowingByUserViewModel
                         {
                             UserId = follow.UserId.ToString(),
                             DateCreated = follow.DateCreated,
                             IsReceiveEmail = follow.IsReceiveEmail,
                             ProviderName = provider.ProviderName,
                             ProviderId = follow.ProviderId.ToString()
                         }).ToList();
            return query;
        }
    }
}
