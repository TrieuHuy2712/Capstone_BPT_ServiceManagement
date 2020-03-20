using System.Threading.Tasks;
using BPT_Service.Application.CategoryService.ViewModel;
using BPT_Service.Application.PermissionService.Query.CheckUserIsAdmin;
using BPT_Service.Application.PermissionService.Query.GetPermissionAction;
using BPT_Service.Common.Helpers;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BPT_Service.Application.CategoryService.Command.DeleteCategoryService
{
    public class DeleteCategoryServiceCommand : IDeleteCategoryServiceCommand
    {
        private readonly IRepository<Category, int> _categoryRepository;
        private readonly IHttpContextAccessor _httpContectAccessor;
        private readonly ICheckUserIsAdminQuery _checkUserIsAdminQuery;
        private readonly IGetPermissionActionQuery _getPermissionActionQuery;
        public DeleteCategoryServiceCommand(
        IRepository<Category, int> categoryRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor,
        ICheckUserIsAdminQuery checkUserIsAdminQuery,
        IGetPermissionActionQuery getPermissionActionQuery)
        {
            _categoryRepository = categoryRepository;
            _httpContectAccessor = httpContextAccessor;
            _checkUserIsAdminQuery = checkUserIsAdminQuery;
            _getPermissionActionQuery = getPermissionActionQuery;
        }
        public async Task<CommandResult<CategoryServiceViewModel>> ExecuteAsync(int id)
        {
            try
            {
                var userId = _httpContectAccessor.HttpContext.User.Identity.Name;
                if( await _checkUserIsAdminQuery.ExecuteAsync(userId) || await _getPermissionActionQuery.ExecuteAsync(userId, "CATEGORY", ActionSetting.CanDelete))
                {
                var CategoryDel = await _categoryRepository.FindByIdAsync(id);
                if (CategoryDel != null)
                {
                    _categoryRepository.Remove(CategoryDel);
                    await _categoryRepository.SaveAsync();
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = true,
                        myModel = new CategoryServiceViewModel
                        {
                            CategoryName = CategoryDel.CategoryName,
                            Description = CategoryDel.Description,
                            Id = CategoryDel.Id
                        }
                    };
                }
                else
                {
                    return new CommandResult<CategoryServiceViewModel>
                    {
                        isValid = false,
                        errorMessage = "Cannot find your Id"
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