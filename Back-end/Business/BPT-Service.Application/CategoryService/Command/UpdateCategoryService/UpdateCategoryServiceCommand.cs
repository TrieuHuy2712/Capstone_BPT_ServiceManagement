using System;
using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.CategoryService.Command.UpdateCategoryService
{
    public class UpdateCategoryServiceCommand : IUpdateCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UpdateCategoryServiceCommand(
        IRepository<Category, int> categoryRepository,
        IUnitOfWork unitOfWork,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery,
        IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(CategoryServiceViewModel userVm)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if(await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "CATEGORY", ActionSetting.CanUpdate))
                {
                    var CategoryUpdate = await _categoryRepository.FindByIdAsync(userVm.Id);
                    if (CategoryUpdate != null)
                    {
                        CategoryUpdate.CategoryName = userVm.CategoryName;
                        CategoryUpdate.Description = userVm.Description;
                        _categoryRepository.Update(CategoryUpdate);
                        await _categoryRepository.SaveAsync();
                        return new CommandResult<CategoryServiceViewModel>
                        {
                            isValid = true,
                            myModel = userVm,
                            errorMessage = "Complete updated"
                        };
                }
                else
                {
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Can't not find ID",
                    };
                }
                }else
                {
                    return new CommandResult<CategoryServiceViewModel>{
                        isValid = false,
                        errorMessage = "You don't have permission"
                    };
                }
            }
            catch (Exception ex)
            {
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    myModel = userVm,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}