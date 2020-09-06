using BPT_Service.Application.LoggingService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.NotificationService.NotificationUser.AutoGetUserNotification
{
    public interface IAutoGetUserNotification
    {
        Task<LogTypeViewModel> Execute();
    }
}