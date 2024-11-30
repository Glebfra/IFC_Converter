using System.Reflection;
using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Start;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;

namespace IFC_Converter;

public static class Program
{
    public static void Main(string[] args)
    {
        // Console.WriteLine("Write a ctp file location: ");
        // string inputFilepath = Console.ReadLine();
        // string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        // Console.WriteLine($"Input file is: {inputFilepath}");

        string inputFilepath = "D:\\Bend.ctp";
        string outputFilepath = "D:\\Bend.ifc";

        using StartProject startProject = new StartProject(inputFilepath);
        using IFCConverter ifcConverter = new IFCConverter("Ifc Project");

        Dictionary<int, IfcNodeEntity> nodeEntities = new Dictionary<int, IfcNodeEntity>();
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        foreach (var startNodeEntity in startNodeEntities)
        {
            Console.WriteLine($"Added node {startNodeEntity.Id}");

            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcConverter.AddEntity(ifcNodeEntity);
            nodeEntities.Add(startNodeEntity.Id, ifcNodeEntity);
        }

        Dictionary<int, IfcPipeEntity> pipeEntities = new Dictionary<int, IfcPipeEntity>();
        StartPipeEntity[] startPipeEntities = startProject.GetEntities<StartPipeEntity>(StartElementType.PIPE_ELEMENT);
        foreach (var startPipeEntity in startPipeEntities)
        {
            Console.WriteLine($"Added pipe {startPipeEntity.Id}");

            IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity);
            ifcConverter.AddEntity(ifcPipeEntity);
            pipeEntities.Add(startPipeEntity.Id, ifcPipeEntity);
        }

        Dictionary<int, IfcBendEntity> bendEntities = new Dictionary<int, IfcBendEntity>();
        StartBendEntity[] startBendEntities = startProject.GetEntities<StartBendEntity>(StartElementType.ELBOW);
        foreach (var startBendEntity in startBendEntities)
        {
            Console.WriteLine($"Added bend {startBendEntity.Id}");

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startBendEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => pipeEntities[item.Id]).ToArray();
            IfcBendEntity ifcBendEntity = new IfcBendEntity(startBendEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcBendEntity);
            bendEntities.Add(startBendEntity.Id, ifcBendEntity);
        }

        Dictionary<int, IfcWeldedTeeEntity> weldedTeeEntities = new Dictionary<int, IfcWeldedTeeEntity>();
        StartWeldedTeeEntity[] startWeldedTeeEntities = startProject.GetEntities<StartWeldedTeeEntity>(StartElementType.WELDED_TEE);
        foreach (var startWeldedTeeEntity in startWeldedTeeEntities)
        {
            Console.WriteLine($"Added welded tee {startWeldedTeeEntity.Id}");

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startWeldedTeeEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => pipeEntities[item.Id]).ToArray();
            IfcWeldedTeeEntity ifcWeldedTeeEntity = new IfcWeldedTeeEntity(startWeldedTeeEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcWeldedTeeEntity);
            weldedTeeEntities.Add(startWeldedTeeEntity.Id, ifcWeldedTeeEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static Dictionary<T, U> ConvertObjects<T, U>(StartProject startProject, IFCConverter ifcConverter, StartElementType type) 
        where T : StartAbstractEntity
        where U : IfcAbstractEntity
    {
        Dictionary<T, U> dictionary = new Dictionary<T, U>();
        var objs = startProject.GetEntities<T>(type);
        foreach (var obj in objs)
        {
            Console.WriteLine($"Added {typeof(T).Name} with Id: {obj.Id}");
            
            U ifcObj = (U)Activator.CreateInstance(typeof(U), obj)!;
            ifcConverter.AddEntity(ifcObj);
            dictionary.Add(obj, ifcObj);
        }

        return dictionary;
    }
}