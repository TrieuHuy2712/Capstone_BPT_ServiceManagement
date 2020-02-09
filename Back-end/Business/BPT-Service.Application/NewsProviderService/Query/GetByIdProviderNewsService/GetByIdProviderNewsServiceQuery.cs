using System;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Query.GetByIdProviderNewsService
{
    public class GetByIdProviderNewsServiceQuery : IGetByIdProviderNewsServiceQuery
    {
        private readonly IRepository<ProviderNew, int> _providerNewsRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetByIdProviderNewsServiceQuery(IRepository<ProviderNew, int> providerNewsRepository,
        IHttpContextAccessor httpContextAccessor)
        {
            _providerNewsRepository = providerNewsRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity;
                var getPro = await _providerNewsRepository.FindByIdAsync(id);
                if (getPro == null)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        myModel = null
                    };
                }
                if (getPro.Provider.AppUser.UserName == userId.Name)
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        myModel = MappingProvider(getPro)
                    };
                }
                else
                {
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = false,
                        errorMessage = "You don't have permission with this id"
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
        private NewsProviderViewModel MappingProvider(ProviderNew vm)
        {
            NewsProviderViewModel pro = new NewsProviderViewModel();
            pro.Id = vm.Id;
            pro.Author = vm.Author;
            pro.Status = vm.Status;
            pro.Author = vm.Author;
            pro.ProviderId = vm.ProviderId;
            pro.Title = vm.Title;
            pro.Content = vm.Content;
            pro.DateModified = DateTime.Now;
            return pro;
        }
    }
}