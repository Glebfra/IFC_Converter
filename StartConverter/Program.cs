using System;
using STARTtoIFC;
using System.IO;
using Start;

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

            using (StartProject startProject = StartProject.OpenProject(filePath))
            {
                /*Array values = Enum.GetValues(typeof(StartElementType));
                foreach (StartElementType value in values)
                {
                    Console.WriteLine($@"{value} : {startProject.GetNumberElements(value, value)}");
                }
                string dataJson = startProject.GetDataJson();
                StartDataArrayItem[]? dataArrayItems = JsonConvert.DeserializeObject<StartDataArrayItem[]>(dataJson);
                if (dataArrayItems == null) throw new NullReferenceException("Object is null");
                foreach (StartDataArrayItem startDataArrayItem in dataArrayItems)
                {
                    StartAbstractEntity? abstractEntity = StartEntityFactory.CreateEntity(startDataArrayItem);
                    if (abstractEntity == null) continue;
                    if (abstractEntity.Type == StartElementType.RIGID_ELEMENT)
                    {
                        StartRigidElementEntity rigidEntity = (StartRigidElementEntity)abstractEntity;
                        foreach (PropertyInfo propertyInfo in rigidEntity.GetType().GetProperties())
                        {
                            Console.WriteLine($"{propertyInfo.Name} : {propertyInfo.GetValue(rigidEntity)}");
                        }
                    }
                }*/
            }
            
            // using (StartAutoServer startAutoServer = new StartAutoServer())
            // {
            //     object? startDocument = startAutoServer.LoadStartDocumentRaw(0x4, filePath);
            //     if (startDocument == null) throw new NullReferenceException("Object ref is null");
            //     int code = ifcExporter.Export(startDocument, 1049);
            //     Console.WriteLine($"Output code: {code}");
            // }
        }
    }
}
