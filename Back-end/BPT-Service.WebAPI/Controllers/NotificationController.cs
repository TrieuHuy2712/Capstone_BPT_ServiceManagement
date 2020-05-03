using BPT_Service.Application.NotificationService.NotificationAdmin.AutoGetNotification;
using BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification;
using BPT_Service.Application.NotificationService.NotificationAdmin.GetNotificationHasRead;
using BPT_Service.Application.NotificationService.NotificationUser.AutoGetUserNotification;
using BPT_Service.Application.NotificationService.NotificationUser.AutoRealTimeUserNotification;
using BPT_Service.Application.NotificationService.NotificationUser.GetUserNotificationHasRead;
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
        [HttpGet("AutoGetNotification")]
        public IActionResult AutoGetNotification()
        {
            _autoGetNotification.Execute();
            return new OkObjectResult(true);
        }

        #endregion

        #region GET NOTIFICATION API

        [HttpGet("RealTimeUserNotification")]
        public IActionResult RealTimeUserNotification()
        {
            var model = _autoRealTimeUserNotification.Execute();
            return new OkObjectResult(model);
        }

        [HttpGet("GetUserNotificationHasRead")]
        public IActionResult GetUserNotificationHasRead()
        {
            _getUserNotificationHasRead.Execute();
            return new OkObjectResult(true);
        }
        [HttpGet("AutoGetUserNotification")]
        public IActionResult AutoGetUserNotification()
        {
            _autoGetUserNotification.Execute();
            return new OkObjectResult(true);
        }

        #endregion GET API
    }
}