using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;

namespace StartConverter;

public static class Program
{
    private static Dictionary<int, IfcNodeEntity> _nodeEntities = new Dictionary<int, IfcNodeEntity>();
    private static Dictionary<int, IfcPipeEntity> _pipeEntities = new Dictionary<int, IfcPipeEntity>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Write a ctp file location: ");
        //string inputFilepath = Console.ReadLine();
        string inputFilepath = "D:\\Работа\\Bend.ctp";
        string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        Console.WriteLine($"Input file is: {inputFilepath}");

        using StartProject startProject = StartProject.OpenProject(inputFilepath);
        using IFCProject ifcProject = IFCProject.CreateProject("StartToIfc");

        AddNodes(startProject, ifcProject);
        AddPipes(startProject, ifcProject);

        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.ELBOW);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.MILTER_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.WELDED_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.LONG_RADIUS_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PRE_STRESSED_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.SADDLE_BEND);

        ConvertDependedObjects<StartMilterJointEntity, IfcMilterJointEntity>(startProject, ifcProject, StartElementType.MILTER_JOINT);
        
        ConvertDependedObjects<StartWeldedTeeEntity, IfcWeldedTeeEntity>(startProject, ifcProject, StartElementType.WELDED_TEE);
        ConvertDependedObjects<StartFabricatedTeeEntity, IfcFabricatedTeeEntity>(startProject, ifcProject, StartElementType.FABRICATED_TEE);
        ConvertDependedObjects<StartStubInEntity, IfcStubInEntity>(startProject, ifcProject, StartElementType.STUB_IN);
        
        ConvertDependedObjects<StartReducerConcentricEntity, IfcReducerConcentricEntity>(startProject, ifcProject, StartElementType.REDUCER_CONCENTRIC);
        ConvertDependedObjects<StartReducerEccentricEntity, IfcReducerEccentricEntity>(startProject, ifcProject, StartElementType.REDUCER_ECCENTRIC);

        ifcProject.GroupObjects("Pipe System");
        ifcProject.SaveAs(outputFilepath);

        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static void AddNodes(StartProject startProject, IFCProject ifcProject)
    {
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        foreach (StartNodeEntity startNodeEntity in startNodeEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added node {startNodeEntity.Id}");
            #endif
            
            IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
            ifcProject.AddEntity(ifcNodeEntity);
            _nodeEntities.Add(startNodeEntity.Id, ifcNodeEntity);
        }
    }

    private static void AddPipes(StartProject startProject, IFCProject ifcProject)
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
            ifcProject.AddEntity(ifcPipeEntity);
            _pipeEntities.Add(startPipeEntity.Id, ifcPipeEntity);
        }
    }

    private static Dictionary<int, U> ConvertBaseObjects<T, U>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : StartAbstractEntity
        where U : IfcAbstractEntity
    {
        Dictionary<int, U> dictionary = new Dictionary<int, U>();
        foreach (T obj in startProject.GetEntities<T>(type))
        {
            Console.WriteLine($"Added {typeof(T).Name} with Id: {obj.Id}");

            U ifcObj = (U)Activator.CreateInstance(typeof(U), obj)!;
            ifcProject.AddEntity(ifcObj);
            dictionary.Add(obj.Id, ifcObj);
        }

        return dictionary;
    }

    private static void ConvertDependedObjects<T, U>(StartProject startProject, IFCProject ifcProject, StartElementType type) 
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
            ifcProject.AddEntity(ifcObj);
        }
    }
}