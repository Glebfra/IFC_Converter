#region

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
        
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.ELBOW);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.MILTER_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.WELDED_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.LONG_RADIUS_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PRE_STRESSED_PIPE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.SADDLE_BEND);
        ConvertDependedObjects<StartBendEntity, IfcMilterJointEntity>(startProject, ifcProject, StartElementType.MILTER_JOINT);
        
        ConvertDependedObjects<StartTeeEntity, IfcWeldedTeeEntity>(startProject, ifcProject, StartElementType.WELDED_TEE);
        ConvertDependedObjects<StartTeeEntity, IfcWeldoletEntity>(startProject, ifcProject, StartElementType.WELDOLET);
        ConvertDependedObjects<StartTeeEntity, IfcSweepoletEntity>(startProject, ifcProject, StartElementType.SWEEPOLET);
        ConvertDependedObjects<StartTeeEntity, IfcFabricatedTeeEntity>(startProject, ifcProject, StartElementType.FABRICATED_TEE);
        ConvertDependedObjects<StartTeeEntity, IfcStubInEntity>(startProject, ifcProject, StartElementType.STUB_IN);
        
        ConvertDependedObjects<StartReducerEntity, IfcReducerConcentricEntity>(startProject, ifcProject, StartElementType.REDUCER_CONCENTRIC);
        ConvertDependedObjects<StartReducerEntity, IfcReducerEccentricEntity>(startProject, ifcProject, StartElementType.REDUCER_ECCENTRIC);
        
        ConvertDependedObjects<StartArmatureEntity, IfcValveEntity>(startProject, ifcProject, StartElementType.VALVE);
        ConvertDependedObjects<StartArmatureEntity, IfcFlangeEntity>(startProject, ifcProject, StartElementType.FLANGE);

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
                U ifcObject = (U)Activator.CreateInstance(typeof(U), startObject, ifcConnNodeEntity, ifcConnPipeEntities)!;
                ifcProject.AddEntity(ifcObject);
            }
        }
    }

    #region PipesAndNodes

    #if PARALLEL
    private static ConcurrentDictionary<int, IfcNodeEntity> AddNodes(StartProject startProject, IFCProject ifcProject)
    {
        StartBaseRoot[] objects = startProject.GetEntities(StartElementType.NODE, StartElementType.NODE);
        ConcurrentDictionary<int, IfcNodeEntity> ifcNodeEntities = new();
        Parallel.ForEach(objects, @object =>
        {
            using (@object)
            {
                #if DEBUG
                Console.WriteLine($"Added StartNodeEntity with Id: {@object.Id}");
                #endif

                StartNodeEntity startNodeEntity = JsonConvert.DeserializeObject<StartNodeEntity>(@object.GetDataJson())!;
                startNodeEntity.XCoord = @object.GetXCoord();
                startNodeEntity.YCoord = @object.GetYCoord();
                startNodeEntity.ZCoord = @object.GetZCoord();
                
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
                ifcNodeEntities.TryAdd(@object.Id, ifcNodeEntity);
            }
        });
        ifcProject.AddEntities(ifcNodeEntities.Values);

        return ifcNodeEntities;
    }
    
    private static ConcurrentDictionary<int, IfcPipeEntity> AddPipes(StartProject startProject, IFCProject ifcProject)
    {
        StartBaseRoot[] objects = startProject.GetEntities(StartElementType.PIPE_ELEMENT, StartElementType.PIPE_ELEMENT);
        ConcurrentDictionary<int, IfcPipeEntity> ifcPipeEntities = new();
        Parallel.ForEach(objects, @object =>
        {
            using (@object)
            {
                #if DEBUG
                Console.WriteLine($"Added StartPipeEntity with Id: {@object.Id}");
                #endif

                StartBaseRoot[] connNodeEntities = startProject.GetConnEntities(@object, StartElementType.NODE);
                IfcNodeEntity[] ifcConnNodeEntities = new IfcNodeEntity[connNodeEntities.Length];
                for (int i = 0; i < connNodeEntities.Length; i++)
                {
                    using (connNodeEntities[i])
                    {
                        ifcConnNodeEntities[i] = _nodeEntities[connNodeEntities[i].Id];
                    }
                }
                
                StartPipeEntity startPipeEntity = JsonConvert.DeserializeObject<StartPipeEntity>(@object.GetDataJson())!;
                startPipeEntity.XCoord = @object.GetXCoord();
                startPipeEntity.YCoord = @object.GetYCoord();
                startPipeEntity.ZCoord = @object.GetZCoord();
                
                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity, ifcConnNodeEntities);
                ifcPipeEntities.TryAdd(@object.Id, ifcPipeEntity);
            }
        });
        ifcProject.AddEntities(ifcPipeEntities.Values);

        return ifcPipeEntities;
    }
    #else
    private static Dictionary<int, IfcNodeEntity> AddNodes(StartProject startProject, IFCProject ifcProject)
    {
        StartBaseRoot[] objects = startProject.GetEntities(StartElementType.NODE, StartElementType.NODE);
        Dictionary<int, IfcNodeEntity> nodeEntities = new Dictionary<int, IfcNodeEntity>();
        foreach (StartBaseRoot @object in objects)
        {
            using (@object)
            {
                #if DEBUG
                Console.WriteLine($"Added StartNodeEntity with Id: {@object.Id}");
                #endif

                StartNodeEntity startNodeEntity = JsonConvert.DeserializeObject<StartNodeEntity>(@object.GetDataJson())!;
                startNodeEntity.XCoord = @object.GetXCoord();
                startNodeEntity.YCoord = @object.GetYCoord();
                startNodeEntity.ZCoord = @object.GetZCoord();
                
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
                ifcProject.AddEntity(ifcNodeEntity);
                nodeEntities.Add(@object.Id, ifcNodeEntity);
            }
        }

        return nodeEntities;
    }

    private static Dictionary<int, IfcPipeEntity> AddPipes(StartProject startProject, IFCProject ifcProject)
    {
        StartBaseRoot[] objects = startProject.GetEntities(StartElementType.PIPE_ELEMENT, StartElementType.PIPE_ELEMENT);
        Dictionary<int, IfcPipeEntity> pipeEntities = new Dictionary<int, IfcPipeEntity>();
        foreach (StartBaseRoot @object in objects)
        {
            using (@object)
            {
                #if DEBUG
                Console.WriteLine($"Added StartPipeEntity with Id: {@object.Id}");
                #endif

                StartBaseRoot[] connNodeEntities = startProject.GetConnEntities(@object, StartElementType.NODE);
                IfcNodeEntity[] ifcConnNodeEntities = new IfcNodeEntity[connNodeEntities.Length];
                for (int i = 0; i < connNodeEntities.Length; i++)
                {
                    using (connNodeEntities[i])
                    {
                        ifcConnNodeEntities[i] = _nodeEntities[connNodeEntities[i].Id];
                    }
                }

                StartPipeEntity startPipeEntity = JsonConvert.DeserializeObject<StartPipeEntity>(@object.GetDataJson())!;
                startPipeEntity.XCoord = @object.GetXCoord();
                startPipeEntity.YCoord = @object.GetYCoord();
                startPipeEntity.ZCoord = @object.GetZCoord();
                
                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity, ifcConnNodeEntities);
                ifcProject.AddEntity(ifcPipeEntity);
                pipeEntities.Add(@object.Id, ifcPipeEntity);
            }
        }

        return pipeEntities;
    }
    #endif

    #endregion
}
