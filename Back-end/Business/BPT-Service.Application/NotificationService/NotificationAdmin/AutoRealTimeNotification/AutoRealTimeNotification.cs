using BPT_Service.Application.LoggingService.Query.GetLogFiles;
using BPT_Service.Application.LoggingService.Query.GetLogFromAFile;
using BPT_Service.Application.LoggingService.ViewModel;
using BPT_Service.Common.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BPT_Service.Application.NotificationService.NotificationAdmin.AutoRealTimeNotification
{
    public class AutoRealTimeNotification : IAutoRealTimeNotification
    {
        //Declare const
        private const string path = "./Logger";

        private const string fileName = "Log-{0}.txt";

        private const string LogNotification = "[ENDNOTIFICATION]\r\n";
        private const string LogSpliter = "[ENDLOG]\r\n";

        private readonly IGetLogFromAFile _getLogFromAFile;
        private readonly IGetLogFiles _getLogFiles;

        public AutoRealTimeNotification(
            IGetLogFromAFile getLogFromAFile,
            IGetLogFiles getLogFiles)
        {
            _getLogFromAFile = getLogFromAFile;
            _getLogFiles = getLogFiles;
        }

        public LogTypeViewModel Execute()
        {
            try
            {
                DateTime datetime = DateTime.Now;
                var currentTime = datetime.ToString(DateFormat.DateFormatStandard);
                var getLogs = GetLogs(currentTime);
                return new LogTypeViewModel
                {
                    NumberNotification = getLogs.Count,
                    Logs = getLogs != null ? getLogs.ToList() : null,
                    Types = "ALL"
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<LogModel> GetLogs(string date)
        {
            string fileName = $"{path}/Log-{date}.txt";
            string content = File.ReadAllText(fileName);

            List<string> logsNotification = content.Split(LogNotification).ToList();
            string contentLog = logsNotification.LastOrDefault();
            if (String.IsNullOrEmpty(content))
            {
                return null;
            }
            List<string> logs = contentLog.Split(LogSpliter).ToList();
            logs.RemoveAt(logs.Count - 1);
            List<LogModel> models = new List<LogModel>();
            foreach (var log in logs)
            {
                LogModel model = new LogModel();
                var splitLog = log.Split("::");
                model.Type = log.Substring(0, log.IndexOf('['));
                model.DateTime = log.Substring(log.IndexOf('[') + 1, log.IndexOf(']') - model.Type.Length - 1);
                string logMessage = splitLog[4].Replace("||", "\n").Trim();
                model.Statement = log.Split("::")[1];
                model.Message = logMessage;
                model.Action = splitLog[2];
                model.UserAction = splitLog[4];
                models.Add(model);
            }
            models.Reverse();
            return models;
        }
    }
}