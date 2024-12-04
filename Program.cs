using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.Start;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;

namespace IFC_Converter;

public static class Program
{
    private static Dictionary<int, IfcNodeEntity>? _nodeEntities;
    private static Dictionary<int, IfcPipeEntity>? _pipeEntities;
    private static Dictionary<int, IfcBendEntity>? _bendEntities = new Dictionary<int, IfcBendEntity>();
    private static Dictionary<int, IfcWeldedTeeEntity>? _weldedTeeEntities = new Dictionary<int, IfcWeldedTeeEntity>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Write a ctp file location: ");
        string inputFilepath = Console.ReadLine();
        string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        Console.WriteLine($"Input file is: {inputFilepath}");

        using StartProject startProject = new StartProject(inputFilepath);
        using IFCConverter ifcConverter = new IFCConverter("Ifc Project");

        _nodeEntities = ConvertObjects<StartNodeEntity, IfcNodeEntity>(startProject, ifcConverter, StartElementType.NODE);
        _pipeEntities = ConvertObjects<StartPipeEntity, IfcPipeEntity>(startProject, ifcConverter, StartElementType.PIPE_ELEMENT);

        StartBendEntity[] startBendEntities = startProject.GetEntities<StartBendEntity>(StartElementType.ELBOW);
        foreach (var startBendEntity in startBendEntities)
        {
            Console.WriteLine($"Added bend {startBendEntity.Id}");

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startBendEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcBendEntity ifcBendEntity = new IfcBendEntity(startBendEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcBendEntity);
            _bendEntities.Add(startBendEntity.Id, ifcBendEntity);
        }
        
        StartWeldedTeeEntity[] startWeldedTeeEntities = startProject.GetEntities<StartWeldedTeeEntity>(StartElementType.WELDED_TEE);
        foreach (var startWeldedTeeEntity in startWeldedTeeEntities)
        {
            Console.WriteLine($"Added welded tee {startWeldedTeeEntity.Id}");

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startWeldedTeeEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcWeldedTeeEntity ifcWeldedTeeEntity = new IfcWeldedTeeEntity(startWeldedTeeEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcWeldedTeeEntity);
            _weldedTeeEntities.Add(startWeldedTeeEntity.Id, ifcWeldedTeeEntity);
        }

        ifcConverter.SaveAs(outputFilepath);
        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static Dictionary<int, U> ConvertObjects<T, U>(StartProject startProject, IFCConverter ifcConverter, StartElementType type)
        where T : StartAbstractEntity
        where U : IfcAbstractEntity
    {
        Dictionary<int, U> dictionary = new Dictionary<int, U>();
        foreach (T obj in startProject.GetEntities<T>(type))
        {
            Console.WriteLine($"Added {typeof(T).Name} with Id: {obj.Id}");
            
            U ifcObj = (U)Activator.CreateInstance(typeof(U), obj)!;
            ifcConverter.AddEntity(ifcObj);
            dictionary.Add(obj.Id, ifcObj);
        }

        return dictionary;
    }
}