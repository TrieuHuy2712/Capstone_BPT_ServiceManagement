using BPT_Service.Common.Constants;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BPT_Service.Common.Logging
{
    public static class Logging<T> where T : class
    {
        private const string INFO = "INFO";
        private const string ERROR = "ERROR";
        private const string WARNING = "WARNING";
        private const string path = "./Logger/";
        private const string fileName = "Log-{0}.txt";

        public static async Task InformationAsync(params string[] message)
        {
            await WritelLogAsync(INFO, message);
        }

        public static async Task InformationAsync(string typeCommand, string userName, params string[] message)
        {
            await WritelLogAsync(INFO, typeCommand, userName, message);
        }

        public static async Task ErrorAsync(params string[] message)
        {
            await WritelLogAsync(ERROR, message);
        }

        public static async Task ErrorAsync(string typeCommand, string userName, params string[] message)
        {
            await WritelLogAsync(ERROR, typeCommand, userName, message);
        }

        public static async Task ErrorAsync(Exception ex, params string[] message)
        {
            await WritelLogAsync(ERROR, string.Join("||", message),
                "Message: " + ex.Message,
                "Inner: " + (ex.InnerException != null ? ex.InnerException.Message : "No inner"),
                "StackTrace: " + (ex.StackTrace != null ? ex.StackTrace : "No StackTrace"),
                "Source: " + (ex.Source != null ? ex.Source : "No Source"));
        }

        public static async Task ErrorAsync(Exception ex, string typeCommand, string userName, params string[] message)
        {
            await WritelLogAsync(ERROR, typeCommand, userName, string.Join("||", message),
                "Message: " + ex.Message,
                "Inner: " + (ex.InnerException != null ? ex.InnerException.Message : "No inner"),
                "StackTrace: " + (ex.StackTrace != null ? ex.StackTrace : "No StackTrace"),
                "Source: " + (ex.Source != null ? ex.Source : "No Source"));
        }

        public static async Task WarningAsync(params string[] message)
        {
            await WritelLogAsync(WARNING, message);
        }

        public static async Task WarningAsync(string typeCommand, string userName, params string[] message)
        {
            await WritelLogAsync(WARNING,typeCommand, userName, message);
        }

        private static async Task WritelLogAsync(string logType, string typeCommand, string userName, params string[] message)
        {
            try
            {
                DateTime datetime = DateTime.Now.AddHours(7);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (StreamWriter fs = File.AppendText(string.Format(path + fileName, datetime.ToString(DateFormat.DateFormatStandard))))
                {
                    var logContent = logType + "[" + datetime.ToString(DateFormat.DateTimeFormat) + "]:: " + typeof(T).Name + ":: " + typeCommand + ":: " + userName + ":: " + string.Join("||", message)+ "[ENDLOG]";
                    await fs.WriteLineAsync(logContent);
                }
            }
            catch (Exception ex)
            {
                await ErrorAsync(ex, message);
            }
        }

        private static async Task WritelLogAsync(string logType, params string[] message)
        {
            try
            {
                DateTime datetime = DateTime.Now.AddHours(7);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (StreamWriter fs = File.AppendText(string.Format(path + fileName, datetime.ToString(DateFormat.DateFormatStandard))))
                {
                    var logContent = logType + "[" + datetime.ToString(DateFormat.DateTimeFormat) + "]:: " + typeof(T).Name + ":: " + string.Join("||", message) + "[ENDLOG]";
                    await fs.WriteLineAsync(logContent);
                }
            }
            catch (Exception ex)
            {
                await ErrorAsync(ex, message);
            }
        }

        public static void Information(params string[] message)
        {
            WritelLog(INFO, message);
        }

        public static void Error(params string[] message)
        {
            WritelLog(ERROR, message);
        }

        public static void Error(Exception ex, params string[] message)
        {
            WritelLog(ERROR, string.Join("||", message),
                "Message: " + ex.Message,
                "Inner: " + (ex.InnerException != null ? ex.InnerException.Message : "No inner"),
                "StackTrace: " + (ex.StackTrace != null ? ex.StackTrace : "No StackTrace"),
                "Source: " + (ex.Source != null ? ex.Source : "No Source"));
        }

        public static void Warning(params string[] message)
        {
            WritelLog(WARNING, message);
        }

        private static void WritelLog(string logType, params string[] message)
        {
            try
            {
                DateTime datetime = DateTime.Now.AddHours(7);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (StreamWriter fs = File.AppendText(string.Format(path + fileName, datetime.ToString(DateFormat.DateFormatStandard))))
                {
                    var logContent = logType + "[" + datetime.ToString(DateFormat.DateTimeFormat) + "]:: " + typeof(T).Name + ":: " + string.Join("||", message) + "[ENDLOG]";
                    fs.WriteLine(logContent);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    Thread.Sleep(5000);
                    WritelLog(logType, message);
                }
                catch
                {
                    Error(ex, message);
                }
            }
        }
    }
}