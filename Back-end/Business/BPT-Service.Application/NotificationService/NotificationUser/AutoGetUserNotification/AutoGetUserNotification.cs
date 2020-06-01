using BPT_Service.Application.LoggingService.ViewModel;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using BPT_Service.Model.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BPT_Service.Application.NotificationService.NotificationUser.AutoGetUserNotification
{
    public class AutoGetUserNotification : IAutoGetUserNotification
    {
        private const string path = "./UserLogger/";

        private const string LogNotification = "[ENDNOTIFICATION]\r\n";
        private const string LogSpliter = "[ENDLOG]\r\n";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AutoGetUserNotification(IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<LogTypeViewModel> Execute()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                LogTypeViewModel logTypeViewModel = new LogTypeViewModel();
                var getListFile = GetAllFile(userId);

                if (getListFile.Any())
                {
                    foreach (var item in getListFile)
                    {
                        var readFile = await GetLogs(item, userId);
                        if (readFile != null)
                        {
                            if (logTypeViewModel.Logs == null)
                            {
                                logTypeViewModel.Logs = readFile;
                            }
                            else
                            {
                                logTypeViewModel.Logs.AddRange(readFile);
                            }
                        }
                    }
                    logTypeViewModel.Types = "ALL";
                    logTypeViewModel.NumberNotification = logTypeViewModel.Logs.Count;
                    return logTypeViewModel;
                }
                return null;
            }
            catch (Exception ex)
            {
                await Logging<AutoGetUserNotification>.ErrorAsync(ex, ActionCommand.COMMAND_NOTIFICATION, "System", "Has error");
                return null;
            }
        }

        private async Task<List<LogModel>> GetLogs(string date, string userId)
        {
            string fileName = $"{path}/{userId}/Log-{date}.txt";
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

        public List<string> GetAllFile(string userid)
        {
            //Get Date only from file name without extension
            List<string> model = new DirectoryInfo(path + "/" + userid + "/").GetFiles().
                Select(x => Path.GetFileNameWithoutExtension(x.Name).Substring(4)).ToList();
            model.Reverse();
            return model;
        }
    }
}