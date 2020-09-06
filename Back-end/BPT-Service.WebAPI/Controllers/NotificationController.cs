using BPT_Service.Application.NotificationService.NotificationAdmin.AutoGetNotification;
using BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification;
using BPT_Service.Application.NotificationService.NotificationAdmin.GetNotificationHasRead;
using BPT_Service.Application.NotificationService.NotificationUser.AutoGetUserNotification;
using BPT_Service.Application.NotificationService.NotificationUser.AutoRealTimeUserNotification;
using BPT_Service.Application.NotificationService.NotificationUser.GetUserNotificationHasRead;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BPT_Service.WebAPI.Controllers
{
    [Route("notification")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly IAutoRealTimeNotification _autorRealTimeNotification;
        private readonly IAutoGetNotification _autoGetNotification;
        private readonly IGetNotificationHasRead _getNotificationHasRead;

        //User Notification
        private readonly IAutoRealTimeUserNotification _autoRealTimeUserNotification;

        private readonly IAutoGetUserNotification _autoGetUserNotification;
        private readonly IGetUserNotificationHasRead _getUserNotificationHasRead;

        public NotificationController(
            IAutoRealTimeNotification autorRealTimeNotification,
            IAutoGetNotification autoGetNotification,
            IGetNotificationHasRead getNotificationHasRead,
            IAutoRealTimeUserNotification autoRealTimeUserNotification,
            IAutoGetUserNotification autoGetUserNotification,
            IGetUserNotificationHasRead getUserNotificationHasRead)
        {
            _autorRealTimeNotification = autorRealTimeNotification;
            _autoGetNotification = autoGetNotification;
            _getNotificationHasRead = getNotificationHasRead;
            _autoRealTimeUserNotification = autoRealTimeUserNotification;
            _autoGetUserNotification = autoGetUserNotification;
            _getUserNotificationHasRead = getUserNotificationHasRead;
        }

        #region GET NOTIFICATION API

        [HttpGet("RealTimeNotification")]
        public async Task<IActionResult> RealTimeNotification()
        {
            var model = await _autorRealTimeNotification.ExecuteAsync();
            return new OkObjectResult(model);
        }

        [HttpGet("GetNotificationHasRead")]
        public IActionResult GetNotificationHasRead()
        {
            _getNotificationHasRead.Execute();
            return new OkObjectResult(true);
        }

        [HttpGet("AutoGetNotification")]
        public async Task<IActionResult> AutoGetNotification()
        {
            var model = await _autoGetNotification.ExecuteAsync();
            return new OkObjectResult(model);
        }

        #endregion GET NOTIFICATION API

        #region GET NOTIFICATION API

        [HttpGet("RealTimeUserNotification")]
        public async Task<IActionResult> RealTimeUserNotification()
        {
            var model = await _autoRealTimeUserNotification.Execute();
            return new OkObjectResult(model);
        }

        [HttpGet("GetUserNotificationHasRead")]
        public IActionResult GetUserNotificationHasRead()
        {
            _getUserNotificationHasRead.Execute();
            return new OkObjectResult(true);
        }

        [HttpGet("AutoGetUserNotification")]
        public async Task<IActionResult> AutoGetUserNotification()
        {
            var model = await _autoGetUserNotification.Execute();
            return new OkObjectResult(model);
        }

        #endregion GET NOTIFICATION API
    }
}