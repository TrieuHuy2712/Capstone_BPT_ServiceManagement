using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BPT_Service.Application.LoggingService.Query.GetLogFiles
{
    public class GetLogFiles : IGetLogFiles
    {
        private const string path = "./Logger";

        public List<string> Execute()
        {
            //Get Date only from file name without extension
            List<string> model = new DirectoryInfo(path).GetFiles().
                Select(x => Path.GetFileNameWithoutExtension(x.Name).Substring(4)).ToList();
            model.Reverse();
            return model;
        }
    }
}