using System.Threading.Tasks;
using BPT_Service.Application.FunctionService.ViewModel;
using BPT_Service.Model.Entities;
using BPT_Service.Model.Infrastructure.Interfaces;

namespace BPT_Service.Application.FunctionService.Command.UpdateFunctionService
{
    public class UpdateFunctionServiceCommand : IUpdateFunctionServiceCommand
    {
        private readonly IRepository<Function, string> _functionRepository;
        public UpdateFunctionServiceCommand(IRepository<Function, string> functionRepository)
        {
            _functionRepository = functionRepository;
        }
        public async Task<bool> ExecuteAsync(FunctionViewModelinFunctionService function)
        {
            var functionDb = await _functionRepository.FindByIdAsync(function.Id);

            if (functionDb != null)
            {
                functionDb.IconCss = function.IconCss;
                functionDb.Id = function.Id;
                functionDb.Name = function.Name;
                functionDb.ParentId = function.ParentId;
                functionDb.SortOrder = function.SortOrder;
                functionDb.Status = function.Status;
                functionDb.URL = function.URL;
                functionDb.NameVietNamese = function.NameVietNamese;
                _functionRepository.Update(functionDb);
                return true;
            }
            return false;
        }
    }
}