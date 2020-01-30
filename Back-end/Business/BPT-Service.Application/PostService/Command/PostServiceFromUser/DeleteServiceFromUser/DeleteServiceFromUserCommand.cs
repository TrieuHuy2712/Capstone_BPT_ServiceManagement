using System;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.PostService.Command.PostServiceFromUser.DeleteServiceFromUser
{
    public class DeleteServiceFromUserCommand : IDeleteServiceFromUserCommand
    {
        private readonly IRepository<Service, Guid> _postServiceRepository;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _userServiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteServiceFromUserCommand(IRepository<Service, Guid> postServiceRepository,
        IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> userServiceRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _postServiceRepository = postServiceRepository;
            _userServiceRepository = userServiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<PostServiceViewModel>> ExecuteAsync(Guid idService)
        {
            try
            {
                var userId = Guid.Parse(_httpContextAccessor.HttpContext.User.Identity.Name);
                var getService = await _postServiceRepository.FindByIdAsync(idService);
                if (getService == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your Service"
                    };
                }

                var getUserService = await _userServiceRepository.FindSingleAsync(x => x.ServiceId == getService.Id);
                if (getUserService == null)
                {
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your ProviderService"
                    };
                }

                if (getUserService.UserId == userId)
                {
                    _postServiceRepository.Remove(getService);
                    await _postServiceRepository.SaveAsync();
                    return new CommandResult<PostServiceViewModel>
                    {
                        isValid = true,
                    };
                }
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find your information"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<PostServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}