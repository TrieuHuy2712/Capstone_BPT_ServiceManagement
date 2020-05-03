using BPT_Service.Application.FollowingPostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPT_Service.Application.FollowingPostService.Query.GetFollowByPost
{
    public class GetFollowByPostQuery : IGetFollowByPostQuery
    {
        private readonly UserManager<AppUser> _userRepository;
        private readonly IRepository<ServiceFollowing, int> _serviceFollwingRepository;
        private readonly IRepository<ServiceImage, int> _imageServiceRepository;

        public GetFollowByPostQuery(
            UserManager<AppUser> userRepository, 
            IRepository<ServiceFollowing, int> serviceFollwingRepository, 
            IRepository<ServiceImage, int> imageServiceRepository)
        {
            _userRepository = userRepository;
            _serviceFollwingRepository = serviceFollwingRepository;
            _imageServiceRepository = imageServiceRepository;
        }

        public async Task<List<ServiceFollowingPostViewModel>> ExecuteAsync(string idService)
        {
            var getAllFollowing = await _serviceFollwingRepository.FindAllAsync(x => x.ServiceId == Guid.Parse(idService));
            var getAllAvartar = await _imageServiceRepository.FindAllAsync(x => x.isAvatar == true);
            var getAllUser = await _userRepository.Users.ToListAsync();
            var data = (from avt in getAllAvartar.ToList()
                        join follow in getAllFollowing.ToList()
                        on avt.ServiceId equals follow.ServiceId
                        join user in getAllUser.ToList()
                        on follow.UserId equals user.Id
                        select new ServiceFollowingPostViewModel
                        {
                            Id = follow.Id,
                            ServiceId = follow.ServiceId.ToString(),
                            DateCreated = follow.DateCreated,
                            UserId = follow.UserId.ToString(),
                            UserName = user.UserName,
                            AvtService = avt.Path
                        }).ToList();
            return data;
        }
    }
}
