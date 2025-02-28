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
            const string filepath = @"D:\Работа\Bend.ctp";
            using (StartAutoServer startAutoServer = new StartAutoServer())
            {
                object? startDocument = startAutoServer.LoadStartDocumentRaw(0x4, filepath);
                if (startDocument == null) throw new NullReferenceException("Object ref is null");
                int code = ifcExporter.Export(startDocument, 1);
                Console.WriteLine($"Output code: {code}");
            }
        }
    }
}
