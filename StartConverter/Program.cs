using System;
using System.Collections.Generic;
using System.Linq;
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
        public static void Main(string[] args)
        {
            #if DEBUG
            DateTime start = DateTime.Now;
            Console.WriteLine($"Started at: {start}");
            #endif
            
            GetFilepath(out string inputFilepath, out string outputFilepath);

            Dictionary<int, IfcNodeEntity> ifcNodeEntities = new Dictionary<int, IfcNodeEntity>();
            Dictionary<int, IfcPipeEntity> ifcPipeEntities = new Dictionary<int, IfcPipeEntity>();
            Dictionary<int, List<IfcPipeEntity>> ifcPipeToNodeRelations = new Dictionary<int, List<IfcPipeEntity>>();
            
            using StartProject startProject = StartProject.OpenProject(inputFilepath);
            using IFCProject ifcProject = IFCProject.CreateProject("StartToIfc");
            
            StartDataArrayItem[] startDataArrayItems = GetArrayData(startProject);
            
            #if DEBUG
            DateTime groupTask = DateTime.Now;
            #endif
            
            GroupObjects(
                startDataArrayItems,
                out Dictionary<int, StartAbstractEntity> nodeEntities,
                out Dictionary<int, StartAbstractEntity> pipeEntities,
                out Dictionary<int, StartAbstractEntity> fittingEntities,
                out Dictionary<int, int[]> pipeNodeRelations,
                out Dictionary<int, int> fittingNodeRelations
            );

            #if DEBUG
            DateTime convertTask = DateTime.Now;
            #endif
            
            foreach (KeyValuePair<int, StartAbstractEntity> nodeEntity in nodeEntities)
            {
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity((StartNodeEntity)nodeEntity.Value);
                ifcNodeEntities.Add(nodeEntity.Key, ifcNodeEntity);
                
                #if DEBUG
                Console.WriteLine($"Added StartNodeEntity with Id: {nodeEntity.Key}");
                #endif
            }
            
            foreach (KeyValuePair<int, StartAbstractEntity> pipeEntity in pipeEntities)
            {
                int[] nodeIds = pipeNodeRelations[pipeEntity.Key];
                IfcNodeEntity[] ifcConnNodeEntities = nodeIds.Select(nodeId => ifcNodeEntities[nodeId]).ToArray();
                IfcPipeEntity ifcPipeEntity = IfcEntityFactory.CreateEntity<IfcPipeEntity>(pipeEntity.Value, ifcConnNodeEntities);
                ifcPipeEntities.Add(pipeEntity.Key, ifcPipeEntity);
                
                foreach (int nodeId in nodeIds)
                {
                    if (!ifcPipeToNodeRelations.ContainsKey(nodeId))
                    {
                        ifcPipeToNodeRelations.Add(nodeId, new List<IfcPipeEntity>());
                    }
                    ifcPipeToNodeRelations[nodeId].Add(ifcPipeEntity);
                }
                
                ifcProject.AddEntity(ifcPipeEntity);
                
                #if DEBUG
                Console.WriteLine($"Added StartPipeEntity with Id: {pipeEntity.Key}");
                #endif
            }

            foreach (KeyValuePair<int, StartAbstractEntity> fittingEntity in fittingEntities)
            {
                int fittingId = fittingEntity.Key;
                StartAbstractEntity fitting = fittingEntity.Value;
                IfcAbstractEntity ifcFittingEntity = IfcEntityFactory.CreateFittingEntity(
                    fitting,
                    ifcNodeEntities[fittingNodeRelations[fittingId]],
                    ifcPipeToNodeRelations[fittingNodeRelations[fittingId]].ToArray()
                );
                ifcProject.AddEntity(ifcFittingEntity);
                
                #if DEBUG
                Console.WriteLine($"Added StartFittingEntity with Id: {fittingEntity.Key}");
                #endif
            }
            
            ifcProject.GroupObjects("Pipe system");
            ifcProject.SaveAs(outputFilepath);
            
            Console.WriteLine($"File saved as: {outputFilepath}");
            
            #if DEBUG
            DateTime end = DateTime.Now;
            Console.WriteLine($"Ended at: {end}");
            Console.WriteLine($"Total time consumed: {end - start}");
            Console.WriteLine($"Group task time consumed: {convertTask - groupTask}");
            Console.WriteLine($"Convert task time consumed: {end - convertTask}");
            #endif
        }
        
        private static StartDataArrayItem[] GetArrayData(StartProject startProject)
        {
            return startProject.GetDataArrayItems() ?? throw new Exception("Data array is null");
        }

        private static void GroupObjects(
            StartDataArrayItem[] startDataArrayItems,
            out Dictionary<int, StartAbstractEntity> nodeEntities,
            out Dictionary<int, StartAbstractEntity> pipeEntities,
            out Dictionary<int, StartAbstractEntity> fittingEntities,
            out Dictionary<int, int[]> pipeNodeRelations,
            out Dictionary<int, int> fittingNodeRelations
        )
        {
            nodeEntities = new Dictionary<int, StartAbstractEntity>();
            pipeEntities = new Dictionary<int, StartAbstractEntity>();
            fittingEntities = new Dictionary<int, StartAbstractEntity>();
            pipeNodeRelations = new Dictionary<int, int[]>();
            fittingNodeRelations = new Dictionary<int, int>();

            foreach (StartDataArrayItem startDataArrayItem in startDataArrayItems)
            {
                StartAbstractEntity? startAbstractEntity = StartEntityFactory.CreateEntity(startDataArrayItem);
                if (startAbstractEntity == null) continue;

                switch (startAbstractEntity.Type)
                {
                    case StartElementType.NODE:
                        nodeEntities.Add(startDataArrayItem.NodeIds[0], startAbstractEntity);
                        break;
                    case StartElementType.PIPE_ELEMENT:
                        pipeEntities.Add(startDataArrayItem.DataArrayIndex, startAbstractEntity);
                        pipeNodeRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds);
                        break;
                    default:
                        fittingEntities.Add(startDataArrayItem.DataArrayIndex, startAbstractEntity);
                        fittingNodeRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds[0]);
                        break;
                }
            }
        }

        private static void GetFilepath(out string inputFilepath, out string outputFilepath)
        {
            Console.Write("Write a ctp file location: ");
            inputFilepath = Console.ReadLine() ?? throw new NullReferenceException("Input filepath cannot be null");

            inputFilepath = inputFilepath.Replace("\"", "");
            outputFilepath = inputFilepath.Replace(".ctp", ".ifc");
            Console.WriteLine($"Input file is: {inputFilepath}");
        }
    }
}
