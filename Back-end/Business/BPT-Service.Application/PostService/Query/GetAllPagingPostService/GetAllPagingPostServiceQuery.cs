using System;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.PostService.Query.GetAllPagingPostService
{
    public class GetAllPagingPostServiceQuery : IGetAllPagingPostServiceQuery
    {
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userManager;
        public GetAllPagingPostServiceQuery(
            IRepository<Service, Guid> serviceRepository,
            UserManager<AppUser> userManager,
        IRepository<Provider, Guid> providerRepository
        )
        {
            _serviceRepository = serviceRepository;
            _providerRepository = providerRepository;
            _userManager = userManager;
        }
        public async Task<PagedResult<ListServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            try
            {
                var query = await _serviceRepository.FindAllAsync();
                if (!string.IsNullOrEmpty(keyword))
                    query = query.Where(x => x.ServiceName.Contains(keyword)
                    || x.Description.Contains(keyword));

                int totalRow = query.Count();
                query = query.Skip((page - 1) * pageSize)
                   .Take(pageSize);

                var data = query.Select(x => new ListServiceViewModel
                {
                    Id = x.Id,
                    CategoryName = x.ServiceCategory.CategoryName,
                    Author = GetProviderInformation(x.ProviderServices.ProviderId)
                            == "" ? GetUserInformation(x.UserServices.UserId) : GetProviderInformation(x.ProviderServices.ProviderId),
                    status = x.Status,
                    isProvider = GetProviderInformation(x.ProviderServices.ProviderId) == "" ? false : true
                }).ToList();

                var paginationSet = new PagedResult<ListServiceViewModel>()
                {
                    Results = data,
                    CurrentPage = page,
                    RowCount = totalRow,
                    PageSize = pageSize
                };

                return paginationSet;
            }
            catch (System.Exception)
            {

                return new PagedResult<ListServiceViewModel>()
                {
                    Results = null,
                    CurrentPage = page,
                    RowCount = 0,
                    PageSize = pageSize
                };
            }
        }

        private string GetProviderInformation(Guid IdProvider)
        {
            if (IdProvider == null)
            {
                return "";
            }
            var getProvider = _providerRepository.FindByIdAsync(IdProvider);
            if (getProvider != null)
            {
                return getProvider.Result.AppUser.FullName + "_Provider: " + getProvider.Result.ProviderName;
            }
            return "";
        }
        private string GetUserInformation(Guid idUser)
        {

            var getUser = _userManager.FindByIdAsync(idUser.ToString());
            if (getUser != null)
            {
                return getUser.Result.FullName;
            }
            return "";
        }
    }
}