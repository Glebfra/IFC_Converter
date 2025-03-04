using System;
using System.IO;
using System.Security.AccessControl;
using Start.API;
using STARTtoIFC;

namespace StartConverter
{
    public static class Program
    {
        private const int _russianCode = 0x0419;
        private const int _englishCode = 0x0409;
        private const int _germanCode = 0x0407;
        
        [STAThread]
        public static void Main(string[] args)
        {
            IfcExporter ifcExporter = new IfcExporter();
            
            #if DEBUG
            string filePath = @"D:\Работа\BendTest.ctp";
            //string filePath = @"C:\Users\nechitailenko\Desktop\CTAPT1.ctp";
            #else
            string filePath;
            while (true)
            {
                Console.WriteLine("Write the path to .ctp file");
                filePath = Console.ReadLine();
                filePath = filePath.Replace("\"", "");
                
                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("FilePath cannot be empty");
                    continue;
                }
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File does not exist");
                    continue;
                }

                break;
            }
            #endif
            
            using (StartAutoServer startAutoServer = new StartAutoServer())
            {
                object? startDocument = startAutoServer.LoadStartDocumentRaw(0x4, filePath);
                if (startDocument == null) throw new NullReferenceException("Object ref is null");
                int code = ifcExporter.Export(startDocument, _russianCode);
                Console.WriteLine($"Output code: {code}");
            }
        }
    }
}
