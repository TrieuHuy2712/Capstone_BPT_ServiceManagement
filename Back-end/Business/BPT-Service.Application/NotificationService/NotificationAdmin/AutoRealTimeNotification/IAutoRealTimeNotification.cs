using BPT_Service.Application.LoggingService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification
{
    //Use on front-end
    //It will run on Angular (setTimeOut) for realTime every minute
    public interface IAutoRealTimeNotification
    {
        Task<LogTypeViewModel> ExecuteAsync();
    }
}