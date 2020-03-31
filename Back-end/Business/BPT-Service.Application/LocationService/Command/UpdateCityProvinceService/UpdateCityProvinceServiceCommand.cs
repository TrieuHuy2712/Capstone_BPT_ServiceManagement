using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.UpdateCityProvinceService
{
    public class UpdateCityProvinceServiceCommand : IUpdateCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _cityProvinceRepository = cityProvinceRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(CityProvinceViewModel vm)
        {
            //Check user has permission
            try
            {//Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "LOCATION", ActionSetting.CanUpdate))
                {
                    var getId = await _cityProvinceRepository.FindByIdAsync(vm.Id);
                    if (getId == null)
                    {
                        return new CommandResult<CityProvinceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    MappingCityProvince(vm, getId);
                    _cityProvinceRepository.Update(getId);
                    await _cityProvinceRepository.SaveAsync();
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = true,
                        myModel = vm
                    };
                }
                else
                {
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_UPDATE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }

        private void MappingCityProvince(CityProvinceViewModel cityProvinceVM, CityProvince cityProvince)
        {
            cityProvinceVM.City = cityProvince.City;
            cityProvinceVM.Province = cityProvince.Province;
        }
    }
}