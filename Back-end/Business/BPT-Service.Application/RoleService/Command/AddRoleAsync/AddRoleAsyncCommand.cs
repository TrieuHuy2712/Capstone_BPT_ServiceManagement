using System.Threading.Tasks;
using BPT_Service.Application.RoleService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace BPT_Service.Application.RoleService.Command.AddRoleAsync
{
    public class AddRoleAsyncCommand : IAddRoleAsyncCommand
    {
        private readonly RoleManager<AppRole> _roleManager;
        private  readonly IUnitOfWork _unitOfWork;
        public AddRoleAsyncCommand(RoleManager<AppRole> roleManager,
        IUnitOfWork unitOfWork){
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> ExecuteAync(AppRoleViewModel roleVm)
        {
           var role = new AppRole()
            {
                Name = roleVm.Name,
                Description = roleVm.Description
            };
            var result = await _roleManager.CreateAsync(role);
            _unitOfWork.Commit();
            return result.Succeeded;
        }
    }
}