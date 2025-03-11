using System;
using System.IO;
using Start.API;
using STARTtoIFC;

namespace StartConverter
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            IfcExporter ifcExporter = new IfcExporter();
            
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

                if (!filePath.EndsWith(".ctp"))
                {
                    Console.WriteLine("Input file should be .ctp formatted");
                    continue;
                }
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File does not exist");
                    continue;
                }

                break;
            }

            using (StartAutoServer startAutoServer = new StartAutoServer())
            {
                object? startDocument = startAutoServer.LoadStartDocumentRaw(0x4, filePath);
                if (startDocument == null) throw new NullReferenceException("Object ref is null");
                int code = ifcExporter.Export(startDocument, 1049);
                Console.WriteLine($"Output code: {code}");
            }
        }
    }
}
