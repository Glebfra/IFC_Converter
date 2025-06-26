using System;
using System.IO;

namespace IfcConverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string ifcFilePath, ctpFilePath;
            while (true)
            {
                Console.WriteLine("Write the path to .ifc file");
                ifcFilePath = Console.ReadLine();
                ifcFilePath = ifcFilePath.Replace("\"", "");
                
                if (string.IsNullOrEmpty(ifcFilePath))
                {
                    Console.WriteLine("FilePath cannot be empty");
                    continue;
                }

                if (!ifcFilePath.EndsWith(".ifc"))
                {
                    Console.WriteLine("Input file should be .ifc formatted");
                    continue;
                }
                
                if (!File.Exists(ifcFilePath))
                {
                    Console.WriteLine("File does not exist");
                    continue;
                }

                break;
            }

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

            IfcConverter ifcConverter = new IfcConverter();
            ifcConverter.Import(ifcFilePath, ctpFilePath);
        }
    }
}