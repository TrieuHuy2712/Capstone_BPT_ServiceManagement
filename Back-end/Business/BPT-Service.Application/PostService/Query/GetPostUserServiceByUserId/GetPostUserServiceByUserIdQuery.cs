using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetPostUserServiceByUserId
{
    public class GetPostUserServiceByUserIdQuery : IGetPostUserServiceByUserIdQuery
    {
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        public GetPostUserServiceByUserIdQuery(
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository)
        {
            _userServiceRepository = userServiceRepository;
        }

        public async Task<ListServiceViewModel> ExecuteAsync(string idUser)
        {
            var findByUserId = await _userServiceRepository.FindSingleAsync(x => x.UserId == Guid.Parse(idUser));
            if (findByUserId == null)
            {
                return null;
            }
            return new ListServiceViewModel
            {
                Id = findByUserId.ServiceId,
            };
        }
    }
}