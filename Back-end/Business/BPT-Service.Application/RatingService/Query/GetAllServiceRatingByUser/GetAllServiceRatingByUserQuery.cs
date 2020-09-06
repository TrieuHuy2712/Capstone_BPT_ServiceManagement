using BPT_Service.Application.ProviderService.Query.CheckUserIsProvider;
using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetAllServiceRatingByUser
{
    public class GetAllServiceRatingByUserQuery : IGetAllServiceRatingByUserQuery
    {
        private readonly ICheckUserIsProviderQuery _checkUserIsProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> _providerServiceRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly UserManager<AppUser> _userRepository;

        public GetAllServiceRatingByUserQuery(
            ICheckUserIsProviderQuery checkUserIsProvider,
            IHttpContextAccessor httpContextAccessor,
            IRepository<Model.Entities.ServiceModel.UserServiceModel.UserService, int> providerServiceRepository,
            IRepository<Service, Guid> serviceRepository,
            IRepository<ServiceRating, int> serviceRatingRepository,
            UserManager<AppUser> userRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
            _checkUserIsProvider = checkUserIsProvider;
            _providerServiceRepository = providerServiceRepository;
        }

        public async Task<PagedResult<ListRatingByServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var checkUserIsProvider = await _checkUserIsProvider.ExecuteAsync(getUserId);
            if (!checkUserIsProvider.isValid)
            {
                return new PagedResult<ListRatingByServiceViewModel>()
                {
                    Results = null,
                    CurrentPage = page,
                    RowCount = 0,
                    PageSize = pageSize
                };
            }
            //Get list service of user
            var getListProviderId = await _providerServiceRepository.FindAllAsync(x => x.UserId);
            var listId = getListProviderId.Select(x => x.ServiceId).ToString();

            var query = GetListServiceOfProvider(getListProviderId.ToList());
            if (query.Count() == 0)
            {
                return new PagedResult<ListRatingByServiceViewModel>()
                {
                    Results = null,
                    CurrentPage = page,
                    RowCount = 0,
                    PageSize = pageSize
                };
            }

            int totalRow = query.Count();
            query = query.Skip((page - 1) * pageSize)
               .Take(pageSize);

            var data = query.Select(x => new ServiceRatingViewModel
            {
                Id = x.Id,
                DateCreated = x.DateCreated,
                NumberOfRating = x.NumberOfRating,
                DateModified = x.DateModified,
                UserNameWithEmail = GetUserAndEmail(x.UserId.ToString()),
                ServiceName = GetNameOfService(x.ServiceId),
                ServiceId = x.ServiceId.ToString()
            }).ToList();

            var group = data.GroupBy(d => new { d.ServiceName, d.ServiceId }).Select(g => new ListRatingByServiceViewModel
            {
                ServiceName = g.Key.ServiceName,
                AverageRating = g.Select(x => x.NumberOfRating).Average(),
                listRating = g.Select(x => new ServiceRatingViewModel
                {
                    NumberOfRating = x.NumberOfRating,
                    UserNameWithEmail = GetUserAndEmail(x.ServiceId),
                    DateCreated = x.DateCreated,
                }).ToList()
            }).ToList();

            var paginationSet = new PagedResult<ListRatingByServiceViewModel>()
            {
                Results = group,
                CurrentPage = page,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return paginationSet;
        }

        private string GetUserAndEmail(string userId)
        {
            var getUser = _userRepository.FindByIdAsync(userId).Result;
            var userMail = getUser.UserName + " (" + getUser.Email + ")";
            return userMail;
        }

        private string GetNameOfService(Guid serviceId)
        {
            var getService = _serviceRepository.FindSingleAsync(x => x.Id == serviceId).Result;
            return getService.ServiceName;
        }

        private IEnumerable<ServiceRating> GetListServiceOfProvider(List<Model.Entities.ServiceModel.UserServiceModel.UserService> userService)
        {
            List<ServiceRating> serv = new List<ServiceRating>();
            foreach (var item in userService)
            {
                var getId = _serviceRatingRepository.FindAllAsync(x=>x.ServiceId == item.ServiceId).Result;
                if(getId != null)
                {
                    serv.AddRange(getId);
                }
            }
            return serv.ToList();
        }
    }
}