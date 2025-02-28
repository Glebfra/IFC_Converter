using System;

namespace STARTtoIFC
{
    public static class Logger
    {
        public static Action<string>? OnLogsChanged;
        
        public static string Logs { get; private set; }
        
        public static void Log(string message)
        {
            string formattedMessage = message + "\n";
            Logs += formattedMessage;
            OnLogsChanged?.Invoke(formattedMessage);
        }
    }
}