using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BPT_Service.Application.ProviderService.ViewModel;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.ProviderService.Query.GetAllProviderofUserService
{
    public class GetAllProviderofUserServiceQuery : IGetAllProviderofUserServiceQuery
    {
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public GetAllProviderofUserServiceQuery(IRepository<Provider, Guid> providerRepository, IHttpContextAccessor httpContextAccessor)
        {
            _providerRepository = providerRepository;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<List<ProviderServiceViewModel>> ExecuteAsync()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getallProviderUser = await _providerRepository.FindAllAsync();
                var getProviderOfUser = getallProviderUser.Where(x => x.UserId == Guid.Parse(userId)).ToList();
                List<ProviderServiceViewModel> listService = getProviderOfUser.Select(x => new ProviderServiceViewModel
                {
                    Id = x.Id.ToString(),
                    ProviderName = x.ProviderName,
                    AvatarPath= x.AvartarPath
                }).ToList();
                return listService.ToList();
            }
            catch (System.Exception ex)
            {

                throw;
            }
        }
    }
}