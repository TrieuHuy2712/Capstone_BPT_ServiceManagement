using BPT_Service.Application.LocationService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BPT_Service.Application.LocationService.Command.DeleteCityProvinceService
{
    public class DeleteCityProvinceServiceCommand : IDeleteCityProvinceServiceCommand
    {
        private readonly IRepository<CityProvince, int> _cityProvinceRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteCityProvinceServiceCommand(IRepository<CityProvince, int> cityProvinceRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _cityProvinceRepository = cityProvinceRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<CityProvinceViewModel>> ExecuteAsync(int id)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                //Check user has permission first
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "LOCATION", ActionSetting.CanDelete))
                {
                    var getById = await _cityProvinceRepository.FindByIdAsync(id);
                    if (getById == null)
                    {
                        await Logging<DeleteCityProvinceServiceCommand>.WarningAsync(ActionCommand.COMMAND_DELETE, ErrorMessageConstant.ERROR_CANNOT_FIND_ID);
                        return new CommandResult<CityProvinceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                    var city_Province = getById.City + "_" + getById.Province;
                    _cityProvinceRepository.Remove(getById);
                    await _cityProvinceRepository.SaveAsync();
                    await Logging<DeleteCityProvinceServiceCommand>.
                        InformationAsync(ActionCommand.COMMAND_DELETE, userName, "Has delete " + city_Province);
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = true,
                        errorMessage = "Has Delete"
                    };
                }
                else
                {
                    await Logging<DeleteCityProvinceServiceCommand>.
                        WarningAsync(ActionCommand.COMMAND_DELETE, ErrorMessageConstant.ERROR_DELETE_PERMISSION);
                    return new CommandResult<CityProvinceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (Exception ex)
            {
                await Logging<DeleteCityProvinceServiceCommand>.ErrorAsync(ex, ActionCommand.COMMAND_DELETE, userName, "Has error");
                return new CommandResult<CityProvinceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.Message.ToString()
                };
            }
        }
    }
}