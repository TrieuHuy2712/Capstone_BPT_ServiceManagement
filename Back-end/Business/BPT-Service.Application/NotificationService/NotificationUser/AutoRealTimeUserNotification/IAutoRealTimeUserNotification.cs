using BPT_Service.Application.LoggingService.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace BPT_Service.Application.NotificationService.NotificationUser.AutoRealTimeUserNotification
{
    public interface IAutoRealTimeUserNotification
    {
        LogTypeViewModel Execute();
    }
}
