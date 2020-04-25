using BPT_Service.Application.LoggingService.ViewModel;

namespace BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification
{
    //Use on front-end
    //It will run on Angular (setTimeOut) for realTime every minute
    public interface IAutoRealTimeNotification
    {
        LogTypeViewModel Execute();
    }
}