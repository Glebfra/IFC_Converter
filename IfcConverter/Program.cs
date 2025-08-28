using System;
using System.IO;
using Start.API;
using STARTtoIFC;

namespace IfcConverter
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            IfcImporter ifcImporter = new IfcImporter();
            
            string ctpFilePath;
            while (true)
            {
                Console.WriteLine("Write the path to .ctp file");
                ctpFilePath = Console.ReadLine();
                ctpFilePath = ctpFilePath.Replace("\"", "");
                
                if (string.IsNullOrEmpty(ctpFilePath))
                {
                    Console.WriteLine("FilePath cannot be empty");
                    continue;
                }

                if (!ctpFilePath.EndsWith(".ctp"))
                {
                    Console.WriteLine("Input file should be .ctp formatted");
                    continue;
                }
                
                if (!File.Exists(ctpFilePath))
                {
                    Console.WriteLine("File does not exist");
                    continue;
                }
                
                break;
            }

            using (StartAutoServer autoServer = new StartAutoServer())
            {
                object? startDocument = autoServer.LoadStartDocumentRaw(0x4, ctpFilePath);
                if (startDocument == null) throw new NullReferenceException("Object ref is null");
                int code = ifcImporter.Import(startDocument, 1049);
                Console.WriteLine($"Output code: {code}");
            }
        }
    }
}