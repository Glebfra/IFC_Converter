using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

namespace Utils
{
    public enum LoggerLevel
    {
        ERROR = 0,
        SYSTEM = 1,
        INFO = 2
    }

    public class Logger
    {
        public static Logger GetInstance() => _instance.Value;
        private static readonly Lazy<Logger> _instance =
            new Lazy<Logger>(() => new Logger());
        
        public const LoggerLevel LoggerLevel = Utils.LoggerLevel.INFO;
        
        private int _countErrors;

        private string Logs { get; set; } = "";

        private Logger()
        {
        }

        private void Flush()
        {
            Logs = "";
        }

        public void Info(string message)
        {
            if (LoggerLevel < LoggerLevel.INFO)
                return;
            string formattedMessage = $"[INFO] [{Assembly.GetCallingAssembly().GetName().Name}] {message} \n";
            Logs += formattedMessage;
        }

        public void System(string message)
        {
            if (LoggerLevel < LoggerLevel.SYSTEM)
                return;
            string formattedMessage = $"[SYSTEM] [{Assembly.GetCallingAssembly().GetName().Name}] {message}\n";
            Logs += formattedMessage;
        }

        public void Error(string message)
        {
            string formattedMessage = $"[ERROR] [{Assembly.GetCallingAssembly().GetName().Name}] {message} \n";
            Logs += formattedMessage;
            _countErrors++;
        }

        [Pure]
        private bool HasErrors()
        {
            return _countErrors != 0;
        }

        private void End()
        {
            Info(HasErrors() ? "Convert ended with errors!" : "Convert ended successfully");
        }

        public void SaveAs(string filePath)
        {
            End();
            using (StreamWriter sw = new(filePath))
            {
                sw.WriteLine(Logs);
            }

            Flush();
        }
    }
}