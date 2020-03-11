using BPT_Service.Application.CategoryService.Query.GetAllAsyncCategoryService;
using BPT_Service.Application.PostService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.PostService.Query.GetAllPagingPostService
{
    public class GetAllPagingPostServiceQuery : IGetAllPagingPostServiceQuery
    {
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<Provider, Guid> _providerRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IGetAllAsyncCategoryServiceQuery _getAllAsyncCategoryServiceQuery;
        private readonly IRepository<Tag, Guid> _tagRepository;
        private readonly IRepository<Model.Entities.ServiceModel.TagService, int> _tagServiceRepository;

        public GetAllPagingPostServiceQuery(
            IRepository<Service, Guid> serviceRepository,
            UserManager<AppUser> userManager,
            IRepository<Provider, Guid> providerRepository,
            IGetAllAsyncCategoryServiceQuery getAllAsyncCategoryServiceQuery,
            IRepository<Tag, Guid> tagRepository,
            IRepository<Model.Entities.ServiceModel.TagService, int> tagServiceRepository)
        {
            _serviceRepository = serviceRepository;
            _providerRepository = providerRepository;
            _userManager = userManager;
            _getAllAsyncCategoryServiceQuery = getAllAsyncCategoryServiceQuery;
            _tagRepository = tagRepository;
            _tagServiceRepository = tagServiceRepository;

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

        private async Task<List<ListServiceViewModel>> MappingServiceCategory(IEnumerable<Service> services, string keyWord)
        {
            var getAllCateogry = await _getAllAsyncCategoryServiceQuery.ExecuteAsync();
            var query = (from serv in services.ToList()
                         join category in getAllCateogry.ToList()
                         on serv.CategoryId equals category.Id
                         where category.CategoryName == keyWord
                         select new ListServiceViewModel
                         {
                             Id = serv.Id,
                             CategoryName = category.CategoryName,
                             Author = GetProviderInformation(serv.ProviderServices.ProviderId)
                             == "" ? GetUserInformation(serv.UserServices.UserId) : GetProviderInformation(serv.ProviderServices.ProviderId),
                             status = serv.Status,
                             isProvider = GetProviderInformation(serv.ProviderServices.ProviderId) == "" ? false : true
                         }).ToList();
            return query;
        }

        //private async Task<List<ListServiceViewModel>> MappingTagService(IEnumerable<Service> services, string keyword)
        //{
        //    var getAllTag = await _tagRepository.FindAllAsync();
        //    var getAllUserTag = await _tagServiceRepository.FindAllAsync();


        //    var data = (from serv in services.ToList()
        //                join ) 
        //}
    }
}