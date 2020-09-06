using BPT_Service.Application.RatingService.ViewModel;
using BPT_Service.Common.Dtos;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Entities.ServiceModel;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.RatingService.Query.GetListAllPagingRatingService
{
    public class GetListAllPagingRatingServiceQuery : IGetListAllPagingRatingServiceQuery
    {
        private readonly IRepository<ServiceRating, int> _serviceRatingRepository;
        private readonly IRepository<Service, Guid> _serviceRepository;
        private readonly UserManager<AppUser> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetListAllPagingRatingServiceQuery(IRepository<ServiceRating, int> serviceRatingRepository,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userRepository,
            IRepository<Service, Guid> serviceRepository)
        {
            _serviceRatingRepository = serviceRatingRepository;
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _serviceRepository = serviceRepository;
        }

        public async Task<PagedResult<ListRatingByServiceViewModel>> ExecuteAsync(string keyword, int page, int pageSize)
        {
            var getUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
            var query = await _serviceRatingRepository.FindAllAsync();

            int totalRow = query.Count();
            if (pageSize != 0)
            {
                query = query.Skip((page - 1) * pageSize)
   .Take(pageSize);
            }

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
    }
}