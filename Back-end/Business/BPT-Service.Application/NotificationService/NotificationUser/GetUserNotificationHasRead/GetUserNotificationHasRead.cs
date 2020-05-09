using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BPT_Service.Application.NotificationService.NotificationUser.GetUserNotificationHasRead
{
    public class GetUserNotificationHasRead : IGetUserNotificationHasRead
    {
        private const string path = "./UserLogger/";

        private const string LogNotification = "[ENDNOTIFICATION]\r\n";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetUserNotificationHasRead(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Execute()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Identity.Name;
                var getAllLogFile = GetAllFile(userId);
                foreach (var item in getAllLogFile)
                {
                    GetLogs(item, userId);
                }
            }
            catch (Exception ex)
            {
                Logging<GetUserNotificationHasRead>.ErrorAsync(ex, ActionCommand.COMMAND_NOTIFICATION, "System", "Has error");
            }
        }

        private void GetLogs(string date, string userId)
        {
            string fileName = $"{path}/{userId}/Log-{date}.txt";
            string content = File.ReadAllText(fileName);
            content = content.Replace(LogNotification, "");
            content += LogNotification;
            File.WriteAllText(fileName, content);
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