using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Reflection;

namespace IFCConverter.Utils.Diagnostics
{
    public enum LoggerLevel
    {
        ERROR = 0,
        SYSTEM = 1,
        INFO = 2
    }

    public class Logger
    {
        public const LoggerLevel Level =
            #if INFO
            LoggerLevel.INFO;
            #elif SYSTEM
            LoggerLevel.SYSTEM;
            #else
            Diagnostics.LoggerLevel.ERROR;
        #endif

        private static readonly Lazy<Logger> _instance = new Lazy<Logger>(() => new Logger());

        private int _countErrors;

        private Logger()
        {
        }

        private string Logs { get; set; } = "";

        public static Logger GetInstance()
        {
            return _instance.Value;
        }

        public void Flush()
        {
            Logs = "";
        }

        public void Info(string message)
        {
            if (Level < LoggerLevel.INFO)
                return;
            string formattedMessage = $"[INFO] [{Assembly.GetCallingAssembly().GetName().Name}] {message} \n";
            Logs += formattedMessage;
        }

        public void System(string message)
        {
            if (Level < LoggerLevel.SYSTEM)
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
        public bool HasErrors()
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
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(Logs);
            }

            Flush();
        }
    }
}