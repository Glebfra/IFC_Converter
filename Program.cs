using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.IFC.Entities.Abstract;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;

namespace IFC_Converter;

public static class Program
{
    private static Dictionary<int, IfcNodeEntity> _nodeEntities = new Dictionary<int, IfcNodeEntity>();
    private static Dictionary<int, IfcPipeEntity> _pipeEntities = new Dictionary<int, IfcPipeEntity>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Write a ctp file location: ");
        //string inputFilepath = Console.ReadLine();
        string inputFilepath = "D:\\Работа\\Reducer.ctp";
        string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        Console.WriteLine($"Input file is: {inputFilepath}");

        using StartProject startProject = StartProject.OpenProject(inputFilepath);
        using IFCConverter ifcConverter = new IFCConverter("StartToIfc");

        AddNodes(startProject, ifcConverter);
        AddPipes(startProject, ifcConverter);
        
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.ELBOW);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.MILTER_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.WELDED_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.LONG_RADIUS_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.PRE_STRESSED_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcConverter, StartElementType.SADDLE_BEND);

        ConvertDependedObjects<StartMilterJointEntity, IfcMilterJointEntity>(startProject, ifcConverter, StartElementType.MILTER_JOINT);
        
        ConvertDependedObjects<StartWeldedTeeEntity, IfcWeldedTeeEntity>(startProject, ifcConverter, StartElementType.WELDED_TEE);
        ConvertDependedObjects<StartFabricatedTeeEntity, IfcFabricatedTeeEntity>(startProject, ifcConverter, StartElementType.FABRICATED_TEE);
        ConvertDependedObjects<StartStubInEntity, IfcStubInEntity>(startProject, ifcConverter, StartElementType.STUB_IN);
        
        ConvertDependedObjects<StartReducerConcentricEntity, IfcReducerConcentricEntity>(startProject, ifcConverter, StartElementType.REDUCER_CONCENTRIC);
        ConvertDependedObjects<StartReducerEccentricEntity, IfcReducerEccentricEntity>(startProject, ifcConverter, StartElementType.REDUCER_ECCENTRIC);

        ifcConverter.GroupObjects("Pipe System");
        ifcConverter.SaveAs(outputFilepath);

        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static void AddNodes(StartProject startProject, IFCConverter ifcConverter)
    {
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        foreach (StartNodeEntity startNodeEntity in startNodeEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added node {startNodeEntity.Id}");
            #endif
            
            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcConverter.AddEntity(ifcNodeEntity);
            _nodeEntities.Add(startNodeEntity.Id, ifcNodeEntity);
        }
    }

    private static void AddPipes(StartProject startProject, IFCConverter ifcConverter)
    {
        StartPipeEntity[] startPipeEntities = startProject.GetEntities<StartPipeEntity>(StartElementType.PIPE_ELEMENT);
        foreach (StartPipeEntity startPipeEntity in startPipeEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added pipe {startPipeEntity.Id}");
            #endif
            
            StartNodeEntity[] connNodeEntities = startProject.GetConnEntities<StartNodeEntity>(startPipeEntity, StartElementType.NODE);
            IfcNodeEntity[] ifcConnNodeEntities = connNodeEntities.Select(item => _nodeEntities[item.Id]).ToArray();

            IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity, ifcConnNodeEntities);
            ifcConverter.AddEntity(ifcPipeEntity);
            _pipeEntities.Add(startPipeEntity.Id, ifcPipeEntity);
        }
    }

    private static Dictionary<int, U> ConvertBaseObjects<T, U>(StartProject startProject, IFCConverter ifcConverter, StartElementType type)
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

    private static void ConvertDependedObjects<T, U>(StartProject startProject, IFCConverter ifcConverter, StartElementType type) 
        where T : StartAbstractEntity
        where U : IfcAbstractEntity
    {
        foreach (T obj in startProject.GetEntities<T>(type))
        {
            #if DEBUG
            Console.WriteLine($"Added {typeof(T).Name} with Id: {obj.Id}");
            #endif
            
            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(obj, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();

            U ifcObj = (U)Activator.CreateInstance(typeof(U), obj, ifcConnNodeEntity, ifcConnPipeEntities)!;
            ifcConverter.AddEntity(ifcObj);
        }
    }
}