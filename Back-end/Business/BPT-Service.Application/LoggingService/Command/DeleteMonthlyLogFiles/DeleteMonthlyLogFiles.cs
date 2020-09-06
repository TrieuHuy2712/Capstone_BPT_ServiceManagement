using BPT_Service.Application.LoggingService.Query.GetLogFiles;
using BPT_Service.Common.Logging;
using System.IO;
using System.Linq;

namespace BPT_Service.Application.LoggingService.Command.DeleteMonthlyLogFiles
{
    public class DeleteMonthlyLogFiles : IDeleteMonthlyLogFiles
    {
        private readonly IGetLogFiles _getLogFiles;
        private const string path = "Logger/";

        public DeleteMonthlyLogFiles(IGetLogFiles getLogFiles)
        {
            _getLogFiles = getLogFiles;
        }

        public void Execute()
        {
            try
            {
                var getListFiles = _getLogFiles.Execute();
                if (getListFiles.Count > 30)
                {
                    var fileDelete = getListFiles.Last();
                    if (File.Exists(Path.Combine(path, "Log-"+fileDelete+".txt")))
                    {
                        // If file found, delete it
                        File.Delete(Path.Combine(path, "Log-" + fileDelete + ".txt"));
                        Logging<DeleteMonthlyLogFiles>.InformationAsync("Had deleted "+ "Log-" + fileDelete + ".txt");
                    }
                }
            }
            catch (IOException ioExp)
            {
                Logging<DeleteMonthlyLogFiles>.ErrorAsync(ioExp.ToString());
            }
        }
    }
}