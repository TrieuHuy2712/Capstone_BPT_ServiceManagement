using BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification;
using BPT_Service.Application.NotificationService.NotificationAdmin.GetNotificationHasRead;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IAutoRealTimeNotification _autorRealTimeNotification;
        private readonly IGetNotificationHasRead _getNotificationHasRead;

        public NotificationController(
            IAutoRealTimeNotification autorRealTimeNotification,
            IGetNotificationHasRead getNotificationHasRead)
        {
            _autorRealTimeNotification = autorRealTimeNotification;
            _getNotificationHasRead = getNotificationHasRead;
        }

        #region GET API

        [HttpGet("RealTimeNotification")]
        public IActionResult RealTimeNotification()
        {
            var model = _autorRealTimeNotification.Execute();
            return new OkObjectResult(model);
        }

        [HttpGet("GetNotificationHasRead")]
        public IActionResult GetNotificationHasRead()
        {
            _getNotificationHasRead.Execute();
            return new OkObjectResult(true);
        }

        #endregion GET API
    }
}