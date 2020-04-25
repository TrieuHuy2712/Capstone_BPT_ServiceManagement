using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetAllPostUserServiceByUserId
{
    public class GetAllPostUserServiceByUserIdQuery : IGetAllPostUserServiceByUserIdQuery
    {
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceImage, int> _imageRepository;

        public GetAllPostUserServiceByUserIdQuery(
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<ServiceImage, int> imageRepository)
        {
            _userServiceRepository = userServiceRepository;
            _serviceRepository = serviceRepository;
            _imageRepository = imageRepository;
        }

        public async Task<List<ListServiceViewModel>> ExecuteAsync(string idUser)
        {
            var findByUserId = await _userServiceRepository.FindAllAsync(x => x.UserId == Guid.Parse(idUser));
            var getAllService = await _serviceRepository.FindAllAsync();
            if (findByUserId == null)
            {
                return null;
            }
            var getIsAvatar = await _imageRepository.FindAllAsync(x => x.isAvatar == true);
            var data = (from user in findByUserId.ToList()
                        join service in getAllService.ToList()
                        on user.ServiceId equals service.Id
                        join avatar in getIsAvatar.ToList()
                        on service.Id equals avatar.ServiceId
                        select new ListServiceViewModel
                        {
                            Id = user.ServiceId,
                            ServiceName = service.ServiceName,
                            AvtService = avatar.Path
                        }).ToList();

            return data;
        }
    }
}