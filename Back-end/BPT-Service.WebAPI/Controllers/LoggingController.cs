using BPT_Service.Application.LoggingService.Query.GetLogFiles;
using BPT_Service.Application.LoggingService.Query.GetLogFromAFile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("logging")]
    [ApiController]
    [Authorize]
    public class LoggingController : ControllerBase
    {
        private readonly IGetLogFiles _getLogFiles;
        private readonly IGetLogFromAFile _getLogFromAFile;

        public LoggingController(
            IGetLogFiles getLogFiles,
            IGetLogFromAFile getLogFromAFile)
        {
            _getLogFiles = getLogFiles;
            _getLogFromAFile = getLogFromAFile;
        }

        [HttpGet("GetLogFiles")]
        public async Task<IActionResult> GetLogFiles()
        {
            var model = _getLogFiles.Execute();
            return new OkObjectResult(model);
        }

        [HttpGet("GetLogFromAFile")]
        public async Task<IActionResult> GetLogFromAFile(string datalog, string type)
        {
            var model = _getLogFromAFile.Execute(datalog, type);
            return new OkObjectResult(model);
        }
    }
}