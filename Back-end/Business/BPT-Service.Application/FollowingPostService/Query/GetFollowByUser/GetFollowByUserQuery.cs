using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Query.GetFollowByUser
{
    public class GetFollowByUserQuery : IGetFollowByUserQuery
    {
        private IRepository<ServiceFollowing, int> _serviceFollowingRepository;
        private IRepository<Service, Guid> _serviceRepository;
        public GetFollowByUserQuery(IRepository<ServiceFollowing, int> serviceFollowingRepository,
            IRepository<Service, Guid> serviceRepository)
        {
            _serviceRepository = serviceRepository;
            _serviceFollowingRepository = serviceFollowingRepository;
        }

        public async Task<List<ServiceFollowingUserViewModel>> ExecuteAsync(string userId)
        {
            var findAllFollowing = await _serviceFollowingRepository.FindAllAsync(x => x.UserId == Guid.Parse(userId));
            var findAllService = await _serviceRepository.FindAllAsync();
            var data =  (from following in findAllFollowing.ToList()
                              join ser in findAllService.ToList()
                              on following.ServiceId equals ser.Id
                              select new ServiceFollowingUserViewModel
                              {
                                  Id = following.Id,
                                  ServiceId = ser.Id.ToString(),
                                  DateCreated = following.DateCreated,
                                  ServiceName = ser.ServiceName
                              }).ToList();
            return data;
        }
    }
}
