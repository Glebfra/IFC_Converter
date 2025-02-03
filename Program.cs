using IFC_Converter.IFC;
using IFC_Converter.IFC.Entities;
using IFC_Converter.IFC.Entities.Abstract;
using IFC_Converter.Start;
using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;
using IFC_Converter.Start.Entities.Abstract;

namespace IFC_Converter;

public static class Program
{
    private static Dictionary<int, IfcNodeEntity> _nodeEntities = new Dictionary<int, IfcNodeEntity>();
    private static Dictionary<int, IfcPipeEntity> _pipeEntities = new Dictionary<int, IfcPipeEntity>();
    private static Dictionary<int, IfcBendEntity> _bendEntities = new Dictionary<int, IfcBendEntity>();
    private static Dictionary<int, IfcWeldedTeeEntity> _weldedTeeEntities = new Dictionary<int, IfcWeldedTeeEntity>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Write a ctp file location: ");
        //string inputFilepath = Console.ReadLine();
        string inputFilepath = "D:\\Работа\\Bend.ctp";
        string outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        Console.WriteLine($"Input file is: {inputFilepath}");

        using StartProject startProject = new StartProject(inputFilepath);
        using IFCConverter ifcConverter = new IFCConverter("StartToIfc");
        
        AddNodes(startProject, ifcConverter);
        
        AddPipes(startProject, ifcConverter);
        
        AddBends(startProject, ifcConverter);
        AddMilterJoints(startProject, ifcConverter);
        
        AddWeldedTees(startProject, ifcConverter);
        AddFabricatedTees(startProject, ifcConverter);
        AddStubIns(startProject, ifcConverter);

        AddReducers(startProject, ifcConverter);

        ifcConverter.GroupObjects("Pipe System");
        ifcConverter.SaveAs(outputFilepath);

        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static void AddNodes(StartProject startProject, IFCConverter ifcConverter)
    {
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        foreach (var startNodeEntity in startNodeEntities)
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
        foreach (var startPipeEntity in startPipeEntities)
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

    private static void AddBends(StartProject startProject, IFCConverter ifcConverter)
    {
        List<StartBendEntity> startBendEntities = new List<StartBendEntity>();
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.PIPE_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.ELBOW));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.MILTER_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.WELDED_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.LONG_RADIUS_PIPE_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.PRE_STRESSED_PIPE_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.SADDLE_BEND));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.WELDOLET));
        startBendEntities.AddRange(startProject.GetEntities<StartBendEntity>(StartElementType.SWEEPOLET));
        foreach (var startBendEntity in startBendEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added bend {startBendEntity.Id}");
            #endif

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startBendEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcBendEntity ifcBendEntity = new IfcBendEntity(startBendEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcBendEntity);
            _bendEntities.Add(startBendEntity.Id, ifcBendEntity);
        }

        foreach (var startBendEntity in startBendEntities)
        {
            Console.WriteLine($"Weight {startBendEntity.GetWeight()}");
        }
    }

    private static void AddMilterJoints(StartProject startProject, IFCConverter ifcConverter)
    {
        StartMilterJointEntity[] startMilterJointEntities = startProject.GetEntities<StartMilterJointEntity>(StartElementType.MILTER_JOINT);
        foreach (var startMilterJointEntity in startMilterJointEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added milter joint {startMilterJointEntity.Id}");
            #endif
            
            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startMilterJointEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
            
            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcMilterJointEntity ifcMilterJointEntity = new IfcMilterJointEntity(startMilterJointEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcMilterJointEntity);
        }
    }

    private static void AddWeldedTees(StartProject startProject, IFCConverter ifcConverter)
    {
        List<StartWeldedTeeEntity> startWeldedTeeEntities = new List<StartWeldedTeeEntity>();
        startWeldedTeeEntities.AddRange(startProject.GetEntities<StartWeldedTeeEntity>(StartElementType.WELDED_TEE));
        foreach (var startWeldedTeeEntity in startWeldedTeeEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added welded tee {startWeldedTeeEntity.Id}");
            #endif

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startWeldedTeeEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcWeldedTeeEntity ifcWeldedTeeEntity = new IfcWeldedTeeEntity(startWeldedTeeEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcWeldedTeeEntity);
            _weldedTeeEntities.Add(startWeldedTeeEntity.Id, ifcWeldedTeeEntity);
        }
    }

    private static void AddFabricatedTees(StartProject startProject, IFCConverter ifcConverter)
    {
        List<StartFabricatedTeeEntity> startFabricatedTeeEntities = new List<StartFabricatedTeeEntity>();
        startFabricatedTeeEntities.AddRange(startProject.GetEntities<StartFabricatedTeeEntity>(StartElementType.FABRICATED_TEE));
        foreach (var startFabricatedTeeEntity in startFabricatedTeeEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added fabricated tee {startFabricatedTeeEntity.Id}");
            #endif

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startFabricatedTeeEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcFabricatedTeeEntity ifcFabricatedTeeEntity = new IfcFabricatedTeeEntity(startFabricatedTeeEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcFabricatedTeeEntity);
        }
    }

    private static void AddStubIns(StartProject startProject, IFCConverter ifcConverter)
    {
        List<StartStubInEntity> startStubInEntities = new List<StartStubInEntity>();
        startStubInEntities.AddRange(startProject.GetEntities<StartStubInEntity>(StartElementType.STUB_IN));
        foreach (var startStubInEntity in startStubInEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added stub in {startStubInEntity.Id}");
            #endif

            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startStubInEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);

            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcStubInEntity ifcStubInEntity = new IfcStubInEntity(startStubInEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcStubInEntity);
        }
    }

    private static void AddReducers(StartProject startProject, IFCConverter ifcConverter)
    {
        List<StartReducerConcentricEntity> startReducerConcentricEntities = new List<StartReducerConcentricEntity>();
        startReducerConcentricEntities.AddRange(startProject.GetEntities<StartReducerConcentricEntity>(StartElementType.REDUCER_CONCENTRIC));
        foreach (var startReducerConcentricEntity in startReducerConcentricEntities)
        {
            #if DEBUG
            Console.WriteLine($"Added concentric reducer {startReducerConcentricEntity.Id}");
            #endif
            
            StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startReducerConcentricEntity, StartElementType.NODE);
            StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
            
            IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
            IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();
            IfcReducerConcentricEntity ifcReducerConcentricEntity = new IfcReducerConcentricEntity(startReducerConcentricEntity, ifcConnNodeEntity, ifcConnPipeEntities);

            ifcConverter.AddEntity(ifcReducerConcentricEntity);
        }
    }

    private static Dictionary<int, U> ConvertObjects<T, U>(StartProject startProject, IFCConverter ifcConverter,
        StartElementType type)
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