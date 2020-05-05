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
        private IRepository<ServiceImage, int> _serviceImageRepository;

        public GetFollowByUserQuery(
            IRepository<ServiceFollowing, int> serviceFollowingRepository, 
            IRepository<Service, Guid> serviceRepository, 
            IRepository<ServiceImage, int> serviceImageRepository)
        {
            _serviceFollowingRepository = serviceFollowingRepository;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
        }

        public async Task<List<ServiceFollowingUserViewModel>> ExecuteAsync(string userId)
        {
            var findAllFollowing = await _serviceFollowingRepository.FindAllAsync(x => x.UserId == Guid.Parse(userId));
            var imageFollowing = await _serviceImageRepository.FindAllAsync(x => x.isAvatar == true);
            var findAllService = await _serviceRepository.FindAllAsync();
            var data =  (from following in findAllFollowing.ToList()
                              join ser in findAllService.ToList()
                              on following.ServiceId equals ser.Id
                              join image in imageFollowing.ToList()
                              on ser.Id equals image.ServiceId
                              select new ServiceFollowingUserViewModel
                              {
                                  Id = following.Id,
                                  ServiceId = ser.Id.ToString(),
                                  DateCreated = following.DateCreated,
                                  ServiceName = ser.ServiceName,
                                  AvtService = image.Path
                              }).ToList();
            return data;
        }
    }
}
