using BPT_Service.Application.LoggingService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.NotificationService.NotificationUser.AutoRealTimeUserNotification
{
    public interface IAutoRealTimeUserNotification
    {
        Task<LogTypeViewModel> Execute();
    }
}