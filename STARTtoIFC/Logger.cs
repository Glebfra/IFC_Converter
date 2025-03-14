using System.IO;

namespace STARTtoIFC
{
    public class Logger
    {
        public string Logs { get; private set; }
        
        private static Logger? _instance;
        
        public static Logger GetInstance()
        {
            return _instance ??= new Logger();
        }

        public Logger()
        {
            Logs = "";
        }

        public void Flush()
        {
            Logs = "";
            _instance = null;
        }

        public void Log(string message)
        {
            string formattedMessage = $"[LOG] {message} \n";
            Logs += formattedMessage;
        }
        
        public void Info(string message)
        {
            string formattedMessage = $"[INFO] {message} \n";
            Logs += formattedMessage;
        }

        public void Error(string message)
        {
            string formattedMessage = $"[ERROR] {message} \n";
            Logs += formattedMessage;
        }

        public void SaveAs(string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.WriteLine(Logs);
            }
            Flush();
        }
    }
}