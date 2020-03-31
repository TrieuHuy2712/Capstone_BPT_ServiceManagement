using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.AddCityProvinceService
{
    public class AddCityProvinceServiceCommand : IAddCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AddCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _cityProvinceRepository = cityProvinceRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<List<CityProvinceViewModel>>> ExecuteAsync(List<CityProvinceViewModel> listAddViewModel)
        {
            try
            {//Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "LOCATION", ActionSetting.CanCreate))
                {
                    //Check User Has Permission Create
                    var listModel = MappingCityProvince(listAddViewModel);
                    await _cityProvinceRepository.Add(listModel);
                    await _cityProvinceRepository.SaveAsync();
                    return new CommandResult<List<CityProvinceViewModel>>
                    {
                        isValid = true,
                        myModel = listModel.Select(x => new CityProvinceViewModel
                        {
                            Id = x.Id,
                            City = x.City,
                            Province = x.Province
                        }).ToList()
                    };
                }
                else
                {
                    return new CommandResult<List<CityProvinceViewModel>>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_ADD_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<List<CityProvinceViewModel>>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private List<CityProvince> MappingCityProvince(List<CityProvinceViewModel> listAddViewModel)
        {
            return listAddViewModel.Select(x => new CityProvince
            {
                Id = x.Id,
                City = x.City,
                Province = x.Province
            }).ToList();
        }
    }
}