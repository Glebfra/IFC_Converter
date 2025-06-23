using System;
using System.IO;

namespace IfcConverter
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string filePath;
            while (true)
            {
                Console.WriteLine("Write the path to .ifc file");
                filePath = Console.ReadLine();
                filePath = filePath.Replace("\"", "");
                
                if (string.IsNullOrEmpty(filePath))
                {
                    Console.WriteLine("FilePath cannot be empty");
                    continue;
                }

                if (!filePath.EndsWith(".ifc"))
                {
                    Console.WriteLine("Input file should be .ifc formatted");
                    continue;
                }
                
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("File does not exist");
                    continue;
                }

                break;
            }

            IfcConverter ifcConverter = new IfcConverter();
            ifcConverter.Import(filePath);
        }
    }
}