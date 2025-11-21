using System;
using System.IO;
using IFCConverter;
using Start.API;

namespace Converter
{
    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            ConvertArguments convertArguments = GetConvertArguments(args);
            
            using StartAutoServer autoServer = new StartAutoServer();
            object? startDocument = autoServer.LoadStartDocumentRaw(0x4, convertArguments.CtpFilePath!);
            if (startDocument == null) throw new NullReferenceException("Object ref is null");
            
            IfcConverter converter = new IfcConverter();
            int result = convertArguments.ConvertType switch
            {
                ConvertTypeEnum.STARTtoIFC => converter.Export(startDocument, 1049),
                ConvertTypeEnum.IFCtoSTART => converter.ImportFromFileOpen(startDocument, 1049),
                _ => throw new ArgumentException("Convert type is not set. Use -T 'export' or 'import'")
            };

            // ReSharper disable once LocalizableElement
            Console.WriteLine($"Output code: {result}");
        }

        private static ConvertArguments GetConvertArguments(string[] args)
        {
            ConvertArguments convertArguments = new ConvertArguments();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == ConvertArguments.ConvertTypeArgument)
                {
                    convertArguments.ConvertType = GetConvertType(args[i + 1]);
                    i++;
                }

                if (args[i] == ConvertArguments.CtpFilePathArgument)
                {
                    convertArguments.CtpFilePath = args[i + 1];
                    i++;
                }
            }
            
            convertArguments.ConvertType ??= GetConvertType();
            convertArguments.CtpFilePath ??= GetCtpFilePath();

            return convertArguments;
        }
        
        private static ConvertTypeEnum GetConvertType()
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine("Please specify the conversion type (export/import): ");
            string? arg = Console.ReadLine();
            if (arg == null)
                throw new ArgumentException("Convert type is not set. Use 'export' or 'import'");
            return GetConvertType(arg);
        }

        private static ConvertTypeEnum GetConvertType(string arg)
        {
            string lowerArg = arg.ToLower();
            if (lowerArg == "export")
                return ConvertTypeEnum.STARTtoIFC;
            if (lowerArg == "import")
                return ConvertTypeEnum.IFCtoSTART;
            throw new ArgumentException($"Unknown argument {arg}. Use 'export' or 'import'");
        }

        private static string GetCtpFilePath()
        {
            string? ctpFilePath;
            while (true)
            {
                // ReSharper disable once LocalizableElement
                Console.WriteLine("Write the path to .ctp file");
                ctpFilePath = Console.ReadLine();
                ctpFilePath = ctpFilePath?.Replace("\"", "");

                if (string.IsNullOrEmpty(ctpFilePath))
                {
                    // ReSharper disable once LocalizableElement
                    Console.WriteLine("FilePath cannot be empty");
                    continue;
                }

                if (!ctpFilePath!.EndsWith(".ctp"))
                {
                    // ReSharper disable once LocalizableElement
                    Console.WriteLine("Input file should be .ctp formatted");
                    continue;
                }
                
                if (!File.Exists(ctpFilePath))
                {
                    // ReSharper disable once LocalizableElement
                    Console.WriteLine("File does not exist");
                    continue;
                }
                
                break;
            }

            return ctpFilePath;
        }
    }
}