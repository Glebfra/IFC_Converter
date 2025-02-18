#region

using System.CodeDom;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
#if PARALLEL
using System.Collections.Concurrent;
#endif

#endregion

namespace StartConverter;

public static class Program
{
    #region Fields

    #if PARALLEL
    private static ConcurrentDictionary<int, IfcNodeEntity> _nodeEntities;
    private static ConcurrentDictionary<int, IfcPipeEntity> _pipeEntities;
    #else
    private static Dictionary<int, IfcNodeEntity> _nodeEntities;
    private static Dictionary<int, IfcPipeEntity> _pipeEntities;
    #endif

    #endregion

    public static void Main(string[] args)
    {
        GetFilepath(out string inputFilepath, out string outputFilepath);

        using StartProject startProject = StartProject.OpenProject(inputFilepath);
        using IFCProject ifcProject = IFCProject.CreateProject("StartToIfc");

        _nodeEntities = AddNodes(startProject, ifcProject);
        _pipeEntities = AddPipes(startProject, ifcProject);

        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.PIPE_BEND);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.ELBOW);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.MILTER_BEND);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.WELDED_BEND);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.LONG_RADIUS_PIPE_BEND);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.PRE_STRESSED_PIPE_BEND);
        AddBends<IfcBendEntity>(startProject, ifcProject, StartElementType.SADDLE_BEND);
        AddBends<IfcMilterJointEntity>(startProject, ifcProject, StartElementType.MILTER_JOINT);
        
        AddTees<IfcWeldedTeeEntity>(startProject, ifcProject, StartElementType.WELDED_TEE);
        AddTees<IfcWeldoletEntity>(startProject, ifcProject, StartElementType.WELDOLET);
        AddTees<IfcSweepoletEntity>(startProject, ifcProject, StartElementType.SWEEPOLET);
        AddTees<IfcFabricatedTeeEntity>(startProject, ifcProject, StartElementType.FABRICATED_TEE);
        AddTees<IfcStubInEntity>(startProject, ifcProject, StartElementType.STUB_IN);
        
        AddReducers<IfcReducerConcentricEntity>(startProject, ifcProject, StartElementType.REDUCER_CONCENTRIC);
        AddReducers<IfcReducerEccentricEntity>(startProject, ifcProject, StartElementType.REDUCER_ECCENTRIC);
        
        AddArmatures<IfcValveEntity>(startProject, ifcProject, StartElementType.VALVE);
        AddArmatures<IfcFlangeEntity>(startProject, ifcProject, StartElementType.FLANGE);

        ifcProject.GroupObjects("Pipe System");
        ifcProject.SaveAs(outputFilepath);
        
        Console.WriteLine($"File saved as: {outputFilepath}");
    }

    private static void GetFilepath(out string inputFilepath, out string outputFilepath)
    {
        Console.Write("Write a ctp file location: ");
        inputFilepath = Console.ReadLine();
        if (inputFilepath == null)
            throw new Exception("Input filepath cannot be null");
        
        inputFilepath = inputFilepath.Replace("\"", "");
        outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
        Console.WriteLine($"Input file is: {inputFilepath}");
    }

    private static void ConvertDependedObjects<T, U>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : StartAbstractEntity
        where U : IfcAbstractEntity
    {
        foreach (StartBaseRoot @object in startProject.GetEntities(type, type))
        {
            bool IsFitting;
            bool IsPipe;
            
            using (@object)
            {
                #if DEBUG
                Console.WriteLine($"Added {typeof(T).Name} with Id: {@object.Id}");
                #endif

                StartBaseRoot connNodeEntity = startProject.GetConnEntity(@object, StartElementType.NODE);
                StartBaseRoot[] connPipeEntities = startProject.GetConnEntities(connNodeEntity, StartElementType.PIPE_ELEMENT);

                IfcNodeEntity ifcConnNodeEntity;
                using (connNodeEntity)
                {
                    ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                }

                IfcPipeEntity[] ifcConnPipeEntities = new IfcPipeEntity[connPipeEntities.Length];
                for (int i = 0; i < connPipeEntities.Length; i++)
                {
                    using (connPipeEntities[i])
                    {
                        ifcConnPipeEntities[i] = _pipeEntities[connPipeEntities[i].Id];
                    }
                }

                T startObject = JsonConvert.DeserializeObject<T>(@object.GetDataJson())!;
                U ifcObject = (U)Activator.CreateInstance(typeof(U), @object, ifcConnNodeEntity, ifcConnPipeEntities)!;
                ifcProject.AddEntity(ifcObject);
            }
        }
    }

    private static void AddBends<T>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : IfcAbstractEntity
    {
        StartBendEntity[] startBendEntities = startProject.GetEntities<StartBendEntity>(type);
        foreach (StartBendEntity startBendEntity in startBendEntities)
        {
            using (startBendEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added StartBendEntity with Id: {startBendEntity.Id}");
                #endif
                
                StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startBendEntity, StartElementType.NODE);
                StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
                IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();

                T ifcObj = (T)Activator.CreateInstance(typeof(T), startBendEntity.Properties, ifcConnNodeEntity, ifcConnPipeEntities);
                ifcProject.AddEntity(ifcObj);
            }
        }
    }

    private static void AddTees<T>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : IfcAbstractEntity
    {
        StartTeeEntity[] startTeeEntities = startProject.GetEntities<StartTeeEntity>(type);
        foreach (StartTeeEntity startTeeEntity in startTeeEntities)
        {
            using (startTeeEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added StartBendEntity with Id: {startTeeEntity.Id}");
                #endif
                
                StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startTeeEntity, StartElementType.NODE);
                StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
                IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();

                T ifcObj = (T)Activator.CreateInstance(typeof(T), startTeeEntity.Properties, ifcConnNodeEntity, ifcConnPipeEntities);
                ifcProject.AddEntity(ifcObj);
            }
        }
    }

    private static void AddReducers<T>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : IfcAbstractEntity
    {
        StartReducerEntity[] startReducerEntities = startProject.GetEntities<StartReducerEntity>(type);
        foreach (StartReducerEntity startReducerEntity in startReducerEntities)
        {
            using (startReducerEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added StartReducerEntity with Id: {startReducerEntity.Id}");
                #endif
                
                StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startReducerEntity, StartElementType.NODE);
                StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
                IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();

                T ifcObj = (T)Activator.CreateInstance(typeof(T), startReducerEntity.Properties, ifcConnNodeEntity, ifcConnPipeEntities);
                ifcProject.AddEntity(ifcObj);
            }
        }
    }

    private static void AddArmatures<T>(StartProject startProject, IFCProject ifcProject, StartElementType type)
        where T : IfcAbstractEntity
    {
        StartArmatureEntity[] startArmatureEntities = startProject.GetEntities<StartArmatureEntity>(type);
        foreach (StartArmatureEntity startArmatureEntity in startArmatureEntities)
        {
            using (startArmatureEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added StartReducerEntity with Id: {startArmatureEntity.Id}");
                #endif
                
                StartNodeEntity connNodeEntity = startProject.GetConnEntity<StartNodeEntity>(startArmatureEntity, StartElementType.NODE);
                StartPipeEntity[] connPipeEntities = startProject.GetConnEntities<StartPipeEntity>(connNodeEntity, StartElementType.PIPE_ELEMENT);
                IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                IfcPipeEntity[] ifcConnPipeEntities = connPipeEntities.Select(item => _pipeEntities[item.Id]).ToArray();

                T ifcObj = (T)Activator.CreateInstance(typeof(T), startArmatureEntity.Properties, ifcConnNodeEntity, ifcConnPipeEntities);
                ifcProject.AddEntity(ifcObj);
            }
        }
    }
    
    #region PipesAndNodes

    #if PARALLEL
    private static ConcurrentDictionary<int, IfcNodeEntity> AddNodes(StartProject startProject, IFCProject ifcProject)
    {
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        ConcurrentDictionary<int, IfcNodeEntity> ifcNodeEntities = new();
        Parallel.ForEach(startNodeEntities, startNodeEntity =>
        {
            using (startNodeEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added node {startNodeEntity.Id}");
                #endif

                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity.Properties);
                ifcNodeEntities.TryAdd(startNodeEntity.Id, ifcNodeEntity);
            }
        });
        ifcProject.AddEntities(ifcNodeEntities.Values);

        return ifcNodeEntities;
    }
    
    private static ConcurrentDictionary<int, IfcPipeEntity> AddPipes(StartProject startProject, IFCProject ifcProject)
    {
        StartPipeEntity[] startPipeEntities = startProject.GetEntities<StartPipeEntity>(StartElementType.PIPE_ELEMENT);
        ConcurrentDictionary<int, IfcPipeEntity> ifcPipeEntities = new();
        Parallel.ForEach(startPipeEntities, startPipeEntity =>
        {
            using (startPipeEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added pipe {startPipeEntity.Id}");
                #endif

                StartNodeEntity[] connNodeEntities = startProject.GetConnEntities<StartNodeEntity>(startPipeEntity, StartElementType.NODE);
                IfcNodeEntity[] ifcConnNodeEntities = connNodeEntities.Select(item => _nodeEntities[item.Id]).ToArray();
                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity.Properties, ifcConnNodeEntities);
                ifcPipeEntities.TryAdd(startPipeEntity.Id, ifcPipeEntity);
            }
        });
        ifcProject.AddEntities(ifcPipeEntities.Values);

        return ifcPipeEntities;
    }
    #else
    private static Dictionary<int, IfcNodeEntity> AddNodes(StartProject startProject, IFCProject ifcProject)
    {
        StartNodeEntity[] startNodeEntities = startProject.GetEntities<StartNodeEntity>(StartElementType.NODE);
        Dictionary<int, IfcNodeEntity> nodeEntities = new Dictionary<int, IfcNodeEntity>();
        foreach (StartNodeEntity startNodeEntity in startNodeEntities)
        {
            using (startNodeEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added node {startNodeEntity.Id}");
                #endif

                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity.Properties);
                ifcProject.AddEntity(ifcNodeEntity);
                nodeEntities.Add(startNodeEntity.Id, ifcNodeEntity);
            }
        }

        return nodeEntities;
    }

    private static Dictionary<int, IfcPipeEntity> AddPipes(StartProject startProject, IFCProject ifcProject)
    {
        StartPipeEntity[] startPipeEntities = startProject.GetEntities<StartPipeEntity>(StartElementType.PIPE_ELEMENT);
        Dictionary<int, IfcPipeEntity> pipeEntities = new Dictionary<int, IfcPipeEntity>();
        foreach (StartPipeEntity startPipeEntity in startPipeEntities)
        {
            using (startPipeEntity)
            {
                #if DEBUG
                Console.WriteLine($"Added pipe {startPipeEntity.Id}");
                #endif

                StartNodeEntity[] connNodeEntities = startProject.GetConnEntities<StartNodeEntity>(startPipeEntity, StartElementType.NODE);
                IfcNodeEntity[] ifcConnNodeEntities = connNodeEntities.Select(item => _nodeEntities[item.Id]).ToArray();

                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity.Properties, ifcConnNodeEntities);
                ifcProject.AddEntity(ifcPipeEntity);
                pipeEntities.Add(startPipeEntity.Id, ifcPipeEntity);
            }
        }

        return pipeEntities;
    }
    #endif

    #endregion
}
