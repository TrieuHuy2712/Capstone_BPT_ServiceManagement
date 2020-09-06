using BPT_Service.Application.LoggingService.ViewModel;
using System.Threading.Tasks;

namespace BPT_Service.Application.NotificationService.NotificationAdmin.AutoGetNotification
{
    //Use on back-end
    //System will use hang-fire after every minute to update notification
    public interface IAutoGetNotification
    {
        Task<LogTypeViewModel> ExecuteAsync();
    }
}