using BPT_Service.Application.LoggingService.Query.GetLogFiles;
using BPT_Service.Common.Helpers;
using BPT_Service.Common.Logging;
using System;
using System.IO;

namespace BPT_Service.Application.NotificationService.NotificationAdmin.GetNotificationHasRead
{
    public class GetNotificationHasRead : IGetNotificationHasRead
    {
        //Declare const
        private const string path = "./Logger";

        private const string LogNotification = "[ENDNOTIFICATION]\r\n";

        private readonly IGetLogFiles _getLogFiles;

        public GetNotificationHasRead(IGetLogFiles getLogFiles)
        {
            _getLogFiles = getLogFiles;
        }

        public void Execute()
        {
            try
            {
                var getAllLogFile = _getLogFiles.Execute();
                foreach (var item in getAllLogFile)
                {
                    GetLogs(item);
                }
            }
            catch (Exception ex)
            {
                Logging<GetNotificationHasRead>.ErrorAsync(ex, ActionCommand.COMMAND_NOTIFICATION, "System", "Has error");
            }
        }

        private void GetLogs(string date)
        {
            string fileName = $"{path}/Log-{date}.txt";
            string content = File.ReadAllText(fileName);
            content = content.Replace(LogNotification, "");
            content += LogNotification;
            File.WriteAllText(fileName, content);
        }


    }
}