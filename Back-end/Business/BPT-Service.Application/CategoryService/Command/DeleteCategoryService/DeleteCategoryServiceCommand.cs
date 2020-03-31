using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace BPT_Service.Application.CategoryService.Command.DeleteCategoryService
{
    public class DeleteCategoryServiceCommand : IDeleteCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DeleteCategoryServiceCommand(IRepository<Category, int> categoryRepository,
            ICheckUserIsAdminQuery checkUserIsAdminQuery,
            IGetPermissionActionQuery getPermissionActionQuery,
            IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(int id)
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                if (await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "CATEGORY", ActionSetting.CanDelete))
                {
                    var categoryDel = await _categoryRepository.FindByIdAsync(id);
                    if (categoryDel != null)
                    {
                        _categoryRepository.Remove(categoryDel);
                        await _categoryRepository.SaveAsync();
                        return new CommandResult<CategoryServiceViewModel>
                        {
                            isValid = true,
                            myModel = new CategoryServiceViewModel
                            {
                                CategoryName = categoryDel.CategoryName,
                                Description = categoryDel.Description,
                                Id = categoryDel.Id
                            }
                        };
                    }
                    else
                    {
                        return new CommandResult<CategoryServiceViewModel>
                        {
                            isValid = false,
                            errorMessage = ErrorMessageConstant.ERROR_CANNOT_FIND_ID
                        };
                    }
                }
                else
                {
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = ErrorMessageConstant.ERROR_DELETE_PERMISSION
                    };
                }
            }
            catch (System.Exception ex)
            {
                return new CommandResult<CategoryServiceViewModel>
                {
                    isValid = false,
                    errorMessage = ex.InnerException.ToString()
                };
            }
        }
    }
}