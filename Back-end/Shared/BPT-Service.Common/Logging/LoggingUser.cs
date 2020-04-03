using BPT_Service.Common.Constants;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BPT_Service.Common.Logging
{
    public static class LoggingUser<T> where T : class
    {
        private const string INFO = "INFO";
        private const string ERROR = "ERROR";
        private const string path = "./UserLogger/";
        private const string fileName = "Log-{0}.txt";

        public static async Task InformationAsync(string userNotification, string userName, params string[] message)
        {
            await WritelLogAsync(INFO, userNotification, userName, message);
        }

        public static async Task ErrorAsync(Exception ex, params string[] message)
        {
            await WritelLogAsync(ERROR, string.Join("||", message),
                "Message: " + ex.Message,
                "Inner: " + (ex.InnerException != null ? ex.InnerException.Message : "No inner"),
                "StackTrace: " + (ex.StackTrace != null ? ex.StackTrace : "No StackTrace"),
                "Source: " + (ex.Source != null ? ex.Source : "No Source"));
        }

        private static async Task WritelLogAsync(string logType, string userNotification, string userName, params string[] message)
        {
            try
            {
                DateTime datetime = DateTime.Now;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (StreamWriter fs = File.AppendText(string.Format(path + fileName, datetime.ToString(DateFormat.DateFormatStandard))))
                {
                    var logContent = logType + "[" + datetime.ToString(DateFormat.DateTimeFormat) + "]:: " + typeof(T).Name + ":: " + userNotification + ":: " + userName + ":: " + string.Join("||", message);
                    await fs.WriteLineAsync(logContent);
                }
            }
            catch (Exception ex)
            {
                await ErrorAsync(ex, message);
            }
        }
    }
}