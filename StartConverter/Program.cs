using System;
using System.Collections.Generic;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;

namespace StartConverter
{
    public static class Program
    {
        private static Dictionary<int, IfcNodeEntity> _nodeEntities;
        private static Dictionary<int, IfcPipeEntity> _pipeEntities;

        public static void Main(string[] args)
        {
            GetFilepath(out string inputFilepath, out string outputFilepath);

            using StartProject startProject = StartProject.OpenProject(inputFilepath);
            using IFCProject ifcProject = IFCProject.CreateProject("StartToIfc");

            _nodeEntities = AddNodes(startProject, ifcProject);
            _pipeEntities = AddPipes(startProject, ifcProject);
        
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PIPE_BEND);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.ELBOW);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.MILTER_BEND);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.WELDED_BEND);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.LONG_RADIUS_PIPE_BEND);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.PRE_STRESSED_PIPE_BEND);
            ConvertPipeFittings<StartBendEntity, IfcBendEntity>(startProject, ifcProject, StartElementType.SADDLE_BEND);
            ConvertPipeFittings<StartBendEntity, IfcMilterJointEntity>(startProject, ifcProject, StartElementType.MILTER_JOINT);
        
            ConvertPipeFittings<StartTeeEntity, IfcWeldedTeeEntity>(startProject, ifcProject, StartElementType.WELDED_TEE);
            ConvertPipeFittings<StartTeeEntity, IfcWeldoletEntity>(startProject, ifcProject, StartElementType.WELDOLET);
            ConvertPipeFittings<StartTeeEntity, IfcSweepoletEntity>(startProject, ifcProject, StartElementType.SWEEPOLET);
            ConvertPipeFittings<StartTeeEntity, IfcFabricatedTeeEntity>(startProject, ifcProject, StartElementType.FABRICATED_TEE);
            ConvertPipeFittings<StartTeeEntity, IfcStubInEntity>(startProject, ifcProject, StartElementType.STUB_IN);
        
            ConvertPipeFittings<StartReducerEntity, IfcReducerConcentricEntity>(startProject, ifcProject, StartElementType.REDUCER_CONCENTRIC);
            ConvertPipeFittings<StartReducerEntity, IfcReducerEccentricEntity>(startProject, ifcProject, StartElementType.REDUCER_ECCENTRIC);
        
            ConvertPipeFittings<StartArmatureEntity, IfcValveEntity>(startProject, ifcProject, StartElementType.VALVE);
            ConvertPipeFittings<StartArmatureEntity, IfcFlangeEntity>(startProject, ifcProject, StartElementType.FLANGE);

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

        private static void ConvertPipeFittings<T, U>(StartProject startProject, IFCProject ifcProject, StartElementType type)
            where T : IStartEntity
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
    }
}
