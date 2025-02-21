using System;
using System.Collections.Generic;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
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
            inputFilepath = Console.ReadLine() ?? throw new NullReferenceException("Input filepath cannot be null");

            inputFilepath = inputFilepath.Replace("\"", "");
            outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
            Console.WriteLine($"Input file is: {inputFilepath}");
        }

        private static void ConvertPipeFittings<T, U>(StartProject startProject, IFCProject ifcProject, StartElementType type)
            where T : StartAbstractEntity
            where U : IfcAbstractEntity
        {
            StartBaseRoot[] startEntities = startProject.GetEntities(type, type);
            foreach (StartBaseRoot startEntity in startEntities)
            {
                #if DEBUG
                Console.WriteLine($"Added {typeof(T).Name} with Id: {startEntity.Id}");
                #endif
                
                StartBaseRoot connNodeEntity = startProject.GetConnEntity(startEntity, StartElementType.NODE);
                StartBaseRoot[] connPipeEntities = startProject.GetConnEntities(connNodeEntity, StartElementType.PIPE_ELEMENT);
                IfcNodeEntity ifcConnNodeEntity = _nodeEntities[connNodeEntity.Id];
                
                IfcPipeEntity[] ifcConnPipeEntities = new IfcPipeEntity[connPipeEntities.Length];
                for (int i = 0; i < connPipeEntities.Length; i++)
                {
                    ifcConnPipeEntities[i] = _pipeEntities[connPipeEntities[i].Id];
                }
                
                T startAbstractEntity = StartAbstractEntity.CreateFromStartObject<T>(startEntity);
                U ifcAbstractEntity = (U)Activator.CreateInstance(typeof(U), startAbstractEntity, ifcConnNodeEntity, ifcConnPipeEntities)!;
                ifcProject.AddEntity(ifcAbstractEntity);
                
                foreach (StartBaseRoot connPipeEntity in connPipeEntities)
                {
                    connNodeEntity.Dispose();
                }
                startEntity.Dispose();
                connNodeEntity.Dispose();
            }
        }
    
        private static Dictionary<int, IfcNodeEntity> AddNodes(StartProject startProject, IFCProject ifcProject)
        {
            StartBaseRoot[] startEntities = startProject.GetEntities(StartElementType.NODE, StartElementType.NODE);
            Dictionary<int, IfcNodeEntity> nodeEntities = new Dictionary<int, IfcNodeEntity>();
            foreach (StartBaseRoot startEntity in startEntities)
            {
                StartNodeEntity startNodeEntity = StartAbstractEntity.CreateFromStartObject<StartNodeEntity>(startEntity);
                startNodeEntity.XCoord = startEntity.GetXCoord();
                startNodeEntity.YCoord = startEntity.GetYCoord();
                startNodeEntity.ZCoord = startEntity.GetZCoord();
                
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
                nodeEntities.Add(startEntity.Id, ifcNodeEntity);
                
                startEntity.Dispose();
            }

            return nodeEntities;
        }

        private static Dictionary<int, IfcPipeEntity> AddPipes(StartProject startProject, IFCProject ifcProject)
        {
            StartBaseRoot[] startEntities = startProject.GetEntities(StartElementType.PIPE_ELEMENT, StartElementType.PIPE_ELEMENT);
            Dictionary<int, IfcPipeEntity> pipeEntities = new Dictionary<int, IfcPipeEntity>();
            foreach (StartBaseRoot startEntity in startEntities)
            {
                #if DEBUG
                Console.WriteLine($"Added StartPipeEntity with Id: {startEntity.Id}");
                #endif

                StartPipeEntity startPipeEntity = StartAbstractEntity.CreateFromStartObject<StartPipeEntity>(startEntity);
                startPipeEntity.XCoord = startEntity.GetXCoord();
                startPipeEntity.YCoord = startEntity.GetYCoord();
                startPipeEntity.ZCoord = startEntity.GetZCoord();
                
                IfcPipeEntity ifcPipeEntity = new IfcPipeEntity(startPipeEntity);
                ifcProject.AddEntity(ifcPipeEntity);
                pipeEntities.Add(startEntity.Id, ifcPipeEntity);
                
                startEntity.Dispose();
            }

            return pipeEntities;
        }
    }
}
