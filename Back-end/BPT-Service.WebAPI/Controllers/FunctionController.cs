using BPT_Service.Application.FunctionService.Command.AddFunctionService;
using BPT_Service.Application.FunctionService.Command.DeleteFunctionService;
using BPT_Service.Application.FunctionService.Command.UpdateFunctionService;
using BPT_Service.Application.FunctionService.Command.UpdateParentId;
using BPT_Service.Application.FunctionService.Query.CheckExistedIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetAllFunctionService;
using BPT_Service.Application.FunctionService.Query.GetAllWithParentIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetByIdFunctionService;
using BPT_Service.Application.FunctionService.Query.GetListFunctionWithPermission;
using BPT_Service.Application.FunctionService.Query.ReOrderFunctionService;
using BPT_Service.Application.FunctionService.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("function")]
    [ApiController]
    [Authorize]
    public class FunctionController : ControllerBase
    {
        #region Initialize

        private readonly IAddFunctionServiceCommand _adddFunctionService;
        private readonly ICheckExistedFunctionServiceQuery _checkExistedFunctionService;
        private readonly IDeleteFunctionServiceCommand _deleteFunctionService;
        private readonly IGetAllFunctionServiceQuery _getallFunctionService;
        private readonly IGetAllWithParentIdFunctionServiceQuery _getallwithParentFunctionService;
        private readonly IGetByIdFunctionServiceQuery _getByIDFunctionService;
        private readonly IGetListFunctionWithPermissionQuery _listwithPermissionFunctionService;
        private readonly IReOrderFunctionServiceQuery _reOrderFunctionService;
        private readonly IUpdateFunctionServiceCommand _updateFunctionService;
        private readonly IUpdateParentIdServiceCommand _updateParentFunctionService;

        public FunctionController(IAddFunctionServiceCommand adddFunctionService,
        ICheckExistedFunctionServiceQuery checkExistedFunctionService,
        IDeleteFunctionServiceCommand deleteFunctionService,
        IGetAllFunctionServiceQuery getallFunctionService,
        IGetAllWithParentIdFunctionServiceQuery getallwithParentFunctionService,
        IGetByIdFunctionServiceQuery getByIDFunctionService,
        IGetListFunctionWithPermissionQuery listwithPermissionFunctionService,
        IReOrderFunctionServiceQuery reOrderFunctionService,
        IUpdateFunctionServiceCommand updateFunctionService,
        IUpdateParentIdServiceCommand updateParentFunctionService)
        {
            _adddFunctionService = adddFunctionService;
            _checkExistedFunctionService = checkExistedFunctionService;
            _deleteFunctionService = deleteFunctionService;
            _getallFunctionService = getallFunctionService;
            _getallwithParentFunctionService = getallwithParentFunctionService;
            _getByIDFunctionService = getByIDFunctionService;
            _listwithPermissionFunctionService = listwithPermissionFunctionService;
            _reOrderFunctionService = reOrderFunctionService;
            _updateFunctionService = updateFunctionService;
            _updateParentFunctionService = updateParentFunctionService;
        }

        #endregion Initialize

        #region  GET API
        [HttpGet("GetAllFillter")]
        public IActionResult GetAllFillter(string filter)
        {
            var model = _getallFunctionService.ExecuteAsync(filter);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAll/{nameUser}")]
        public async Task<IActionResult> GetAll(string nameUser)
        {
            var model = await _getallFunctionService.ExecuteAsync(string.Empty);
            var rootFunctions = model.Where(c => c.ParentId == null);
            var items = new List<FunctionViewModelinFunctionService>();
            if (nameUser == "admin")
            {
                foreach (var function in rootFunctions)
                {
                    //add the parent category to the item list
                    items.Add(function);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentId(model.ToList(), function, items);
                }
                var result = items.GroupBy(t => t.ParentId).Select(group => new
                {
                    group.Key,
                    ChildrenId = group.ToList()
                }).ToList();
                return new ObjectResult(result);
            }
            else
            {
                var getListFunction = await _listwithPermissionFunctionService.ExecuteAsync(nameUser);
                foreach (var function in getListFunction)
                {
                    //add the parent category to the item list
                    items.Add(function);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentId(model.ToList(), function, items);
                }
                var result = items.Where(x => x.ParentId != null).GroupBy(t => t.ParentId).Select(group => new
                {
                    group.Key,
                    ChildrenId = group.ToList()
                }).ToList();
                return new ObjectResult(result);
            }
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var model = await _getByIDFunctionService.ExecuteAsync(id);

            return new ObjectResult(model);
        }
        #endregion

        #region POST API
        [HttpPost("addEntity")]
        public async Task<IActionResult> SaveEntity([FromBody]FunctionViewModelinFunctionService functionVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _adddFunctionService.ExecuteAsync(functionVm);
                return new OkObjectResult(execute);
            }
        }

        [HttpPost("UpdateParentId/{sourceId}/{targetId}")]
        public async Task<ActionResult> UpdateParentId(string sourceId, string targetId, [FromBody]Dictionary<string, int> items)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                if (sourceId == targetId)
                {
                    return new BadRequestResult();
                }
                else
                {
                    var execute = await _updateParentFunctionService.ExecuteAsync(sourceId, targetId, items);
                    return new OkObjectResult(execute);
                }
            }
        }

        [HttpPost("ReOrder/{sourceId}/{targetId}")]
        public async Task<IActionResult> ReOrder(string sourceId, string targetId)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }
            else
            {
                if (sourceId == targetId)
                {
                    return new BadRequestResult();
                }
                else
                {
                    var execute = await _reOrderFunctionService.ExecuteAsync(sourceId, targetId);
                    return new OkObjectResult(execute);
                }
            }
        }
        #endregion

        #region PUT API
        [HttpPut("updateEntity")]
        public async Task<IActionResult> updateEntity([FromBody]FunctionViewModelinFunctionService functionVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                var execute = await _updateFunctionService.ExecuteAsync(functionVm);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region DELETE API
        [HttpDelete("DeleteFunction")]
        public async Task<IActionResult> Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                var execute = await _deleteFunctionService.ExecuteAsync(id);
                return new OkObjectResult(execute);
            }
        }
        #endregion

        #region Private Functions
        private void GetByParentId(List<FunctionViewModelinFunctionService> allFunctions,
            FunctionViewModelinFunctionService parent, IList<FunctionViewModelinFunctionService> items)
        {
            //var functionsEntities = allFunctions as FunctionViewModelinFunctionService[] ?? allFunctions.ToArray();
            var subFunctions = allFunctions.Where(c => c.ParentId == parent.Id);
            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(allFunctions, cat, items);
            }
        }
        #endregion
    }
}