using BPT_Service.Application.LoggingService.Query.GetLogFiles;
using BPT_Service.Application.LoggingService.Query.GetLogFromAFile;
using BPT_Service.Application.LoggingService.ViewModel;
using BPT_Service.Common.Constants;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        private readonly UserManager<AppUser> _userManager;

        public AutoRealTimeNotification(IGetLogFromAFile getLogFromAFile, IGetLogFiles getLogFiles, UserManager<AppUser> userManager)
        {
            _getLogFromAFile = getLogFromAFile;
            _getLogFiles = getLogFiles;
            _userManager = userManager;
        }

        public async Task<LogTypeViewModel> ExecuteAsync()
        {
            try
            {
                DateTime datetime = DateTime.Now;
                var currentTime = datetime.ToString(DateFormat.DateFormatStandard);
                var getLogs = await GetLogs(currentTime);
                return new LogTypeViewModel
                {
                    NumberNotification = getLogs.Count,
                    Logs = getLogs != null ? getLogs.ToList() : null,
                    Types = "ALL"
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<List<LogModel>> GetLogs(string date)
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
                if (splitLog.Count() >= 5)
                {
                    model.Type = log.Substring(0, log.IndexOf('['));
                    model.DateTime = log.Substring(log.IndexOf('[') + 1, log.IndexOf(']') - model.Type.Length - 1);
                    string logMessage = splitLog[4].Replace("||", "\n").Trim();
                    model.Statement = log.Split("::")[1];
                    model.Message = logMessage;
                    model.Action = splitLog[2];
                    model.UserAction = splitLog[3];
                    var getAvt = await _userManager.FindByNameAsync(model.UserAction);
                    model.ImageUserAction = getAvt == null ? "" : getAvt.Avatar;
                    models.Add(model);
                }
            }
            models.Reverse();
            return models;
        }
    }
}