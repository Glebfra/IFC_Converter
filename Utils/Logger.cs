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
        public static LoggerLevel LoggerLevel = LoggerLevel.SYSTEM;

        private static Logger? _instance;
        private int _countErrors;

        private Logger()
        {
        }

        private string Logs { get; set; } = "";

        public static Logger GetInstance()
        {
            return _instance ??= new Logger();
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