using BPT_Service.Application.LoggingService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BPT_Service.Application.LoggingService.Query.GetLogFromAFile
{
    public class GetLogFromAFile : IGetLogFromAFile
    {
        private const string path = "./Logger";
        private const string LogSpliter = "[ENDLOG]\r\n";

        public LogTypeViewModel Execute(string dateLog, string typeError)
        {
            try
            {
                LogTypeViewModel model = new LogTypeViewModel();
                model.Logs = GetLogs(dateLog);
                model.Types = typeError;
                //Filter with 1 type or all
                if (typeError != "ALL")
                    model.Logs = model.Logs.Where(x => x.Type == typeError).ToList();
                return model;
            }
            catch (System.Exception ex)
            {

                Logging<GetLogFromAFile>.ErrorAsync(ex, ActionCommand.COMMAND_CONFIRM,"None","Had error");
                throw;
            }
            
        }

        private List<LogModel> GetLogs(string date)
        {
            string fileName = $"{path}/Log-{date}.txt";
            string content = File.ReadAllText(fileName);
            List<string> logs = content.Split(LogSpliter).ToList();
            logs.RemoveAt(logs.Count - 1);

            List<LogModel> models = new List<LogModel>();
            foreach (var log in logs)
            {
                LogModel model = new LogModel();
                var splitLog = log.Split("::");
                if(splitLog.Count() >= 5)
                {
                    model.Type = log.Substring(0, log.IndexOf('['));
                    model.DateTime = log.Substring(log.IndexOf('[') + 1, log.IndexOf(']') - model.Type.Length - 1);
                    string logMessage = splitLog[4].Replace("||", "\n").Trim();
                    model.Statement = log.Split("::")[1];
                    model.Message = logMessage;
                    model.Action = splitLog[2];
                    model.UserAction = splitLog[3];
                    models.Add(model);
                }

            }
            models.Reverse();
            return models;
        }
    }
}