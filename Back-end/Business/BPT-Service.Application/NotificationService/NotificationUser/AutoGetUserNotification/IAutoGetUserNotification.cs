using BPT_Service.Application.LoggingService.ViewModel;

namespace BPT_Service.Application.NotificationService.NotificationUser.AutoGetUserNotification
{
    public interface IAutoGetUserNotification
    {
        LogTypeViewModel Execute();
    }
}