using BPT_Service.Application.LoggingService.ViewModel;

namespace BPT_Service.Application.LoggingService.Query.GetLogFromAFile
{
    public interface IGetLogFromAFile
    {
        LogTypeViewModel Execute(string dateLog, string typeError);
    }
}