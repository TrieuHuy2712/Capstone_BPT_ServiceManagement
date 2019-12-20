using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BPT_Service.Application.Interfaces;
using BPT_Service.Application.ViewModels.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("function")]
    [ApiController]
    [Authorize]
    public class FunctionController : ControllerBase
    {
        #region Initialize

        private IFunctionService _functionService;

        public FunctionController(IFunctionService functionService)
        {
            this._functionService = functionService;
        }

        #endregion Initialize

        [HttpGet("GetAllFillter")]
        public IActionResult GetAllFillter(string filter)
        {
            var model = _functionService.GetAll(filter);
            return new OkObjectResult(model);
        }

        [HttpGet("GetAll/{nameUser}")]
        public async Task<IActionResult> GetAll(string nameUser)
        {
            var model = await _functionService.GetAll(string.Empty);
            var rootFunctions = model.Where(c => c.ParentId == null);
            var items = new List<FunctionViewModel>();
            if (nameUser == "admin")
            {
                foreach (var function in rootFunctions)
                {
                    //add the parent category to the item list
                    items.Add(function);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentId(model.ToList(), function, items);
                }
                var result = items.Where(x => x.ParentId != null).GroupBy(t => t.ParentId).Select(group => new
                {
                    ParentId = group.Key,
                    ChildrenId = group.ToList()
                }).ToList();
                return new ObjectResult(result);
            }else{
                var getListFunction = await _functionService.GetListFunctionWithPermission(nameUser);
                foreach (var function in getListFunction)
                {
                    //add the parent category to the item list
                    items.Add(function);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentId(model.ToList(), function, items);
                }
                var result = items.Where(x => x.ParentId != null).GroupBy(t => t.ParentId).Select(group => new
                {
                    ParentId = group.Key,
                    ChildrenId = group.ToList()
                }).ToList();
                return new ObjectResult(result);
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetById(string id)
        {
            var model = _functionService.GetById(id);

            return new ObjectResult(model);
        }

        [HttpPost("SaveEntity")]
        public IActionResult SaveEntity([FromBody]FunctionViewModel functionVm)
        {
            if (!ModelState.IsValid)
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                return new BadRequestObjectResult(allErrors);
            }
            else
            {
                if (string.IsNullOrWhiteSpace(functionVm.Id))
                {
                    _functionService.Add(functionVm);
                }
                else
                {
                    _functionService.Update(functionVm);
                }
                _functionService.Save();
                return new OkObjectResult(functionVm);
            }
        }

        [HttpPost("UpdateParentId/{sourceId}/{targetId}")]
        public IActionResult UpdateParentId(string sourceId, string targetId, [FromBody]Dictionary<string, int> items)
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
                    _functionService.UpdateParentId(sourceId, targetId, items);
                    _functionService.Save();
                    return new OkResult();
                }
            }
        }

        [HttpPost("ReOrder/{sourceId}/{targetId}")]
        public IActionResult ReOrder(string sourceId, string targetId)
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
                    _functionService.ReOrder(sourceId, targetId);
                    _functionService.Save();
                    return new OkObjectResult(sourceId);
                }
            }
        }

        [HttpPost("DeleteFunction/{id}")]
        public IActionResult Delete(string id)
        {
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            else
            {
                _functionService.Delete(id);
                _functionService.Save();
                return new OkObjectResult(id);
            }
        }

        #region Private Functions
        private void GetByParentId(IEnumerable<FunctionViewModel> allFunctions,
            FunctionViewModel parent, IList<FunctionViewModel> items)
        {
            var functionsEntities = allFunctions as FunctionViewModel[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.Id);
            foreach (var cat in subFunctions)
            {
                //add this category
                items.Add(cat);
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, cat, items);
            }
        }
        #endregion
    }
}