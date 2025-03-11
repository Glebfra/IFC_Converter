using System;
using System.IO;

namespace STARTtoIFC
{
    public static class Logger
    {
        public static Action<string>? OnLogsChanged;
        
        public static string Logs { get; private set; }

        public static void Initialize()
        {
            Logs = "";
        }
        
        public static void Log(string message)
        {
            string formattedMessage = $"[LOG] {message} \n";
            Logs += formattedMessage;
            OnLogsChanged?.Invoke(formattedMessage);
        }

        public static void Error(string message)
        {
            string formattedMessage = $"[ERROR] {message} \n";
            Logs += formattedMessage;
            OnLogsChanged?.Invoke(formattedMessage);
        }

        public static void SaveAs(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Write(Logs);
            }
        }
    }
}