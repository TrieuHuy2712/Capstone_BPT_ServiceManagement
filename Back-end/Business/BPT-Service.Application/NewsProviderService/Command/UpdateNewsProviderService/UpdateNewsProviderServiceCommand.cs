using System;
using System.Threading.Tasks;
using BPT_Service.Application.NewsProviderService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel.ProviderServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.NewsProviderService.Command.UpdateNewsProviderService
{
    public class UpdateNewsProviderServiceCommand : IUpdateNewsProviderServiceCommand
    {
        private readonly IRepository<ProviderNew, int> _providerNewsRepository;
        private readonly IHttpContextAccessor _httpContext;
        public UpdateNewsProviderServiceCommand(IRepository<ProviderNew, int> providerNewsRepository, IHttpContextAccessor httpContext)
        {
            _providerNewsRepository = providerNewsRepository;
            _httpContext = httpContext;
        }
        public async Task<CommandResult<NewsProviderViewModel>> ExecuteAsync(NewsProviderViewModel vm)
        {
            try
            {
                var findProviderNew = await _providerNewsRepository.FindByIdAsync(vm.Id);
                if (findProviderNew != null)
                {
                    var mappingNewsProvider = MappingProvider(vm);
                    _providerNewsRepository.Update(mappingNewsProvider);
                    await _providerNewsRepository.SaveAsync();
                    return new CommandResult<NewsProviderViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = "Cannot find Id"
                };
            }
            catch (System.Exception ex)
            {
                return new CommandResult<NewsProviderViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
        
        private ProviderNew MappingProvider(NewsProviderViewModel vm)
        {
            ProviderNew pro = new ProviderNew();
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