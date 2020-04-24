using System.Collections.Generic;
using System.Threading.Tasks;

namespace BPT_Service.Application.LoggingService.Query.GetLogFiles
{
    public interface IGetLogFiles
    {
        List<string> Execute();
    }
}