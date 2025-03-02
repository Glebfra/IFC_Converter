using System;
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
            //string filePath = @"D:\Работа\Bend.ctp";
            string filePath = @"C:\Users\nechitailenko\Desktop\CTAPT1.ctp";
            using (StartAutoServer startAutoServer = new StartAutoServer())
            {
                object? startDocument = startAutoServer.LoadStartDocumentRaw(0x4, filePath);
                if (startDocument == null) throw new NullReferenceException("Object ref is null");
                int code = ifcExporter.Export(startDocument, 1);
                Console.WriteLine($"Output code: {code}");
            }
        }
    }
}
