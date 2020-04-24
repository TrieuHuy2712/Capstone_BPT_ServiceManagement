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
        public GetAllPostUserServiceByUserIdQuery(
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
            IRepository<Service, Guid> serviceRepository)
        {
            _userServiceRepository = userServiceRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<List<ListServiceViewModel>> ExecuteAsync(string idUser)
        {
            var findByUserId = await _userServiceRepository.FindAllAsync(x => x.UserId == Guid.Parse(idUser));
            var getAllService= await _serviceRepository.FindAllAsync();
            if (findByUserId == null)
            {
                return null;
            }
            var data = from us in findByUserId.ToList()
                        join serv in getAllService.ToList()
                        on us.ServiceId  equals serv.Id
                        select new  


            
            return data;
        }
    }
}