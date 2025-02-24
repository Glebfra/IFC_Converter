using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using Start;
using Start.API;
using Start.Entities;

namespace Tests
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string inputFilepath = @"D:\Работа\Bend.ctp";
            const string outputFilepath = @"D:\Работа\Bend.ifc";
            
            Dictionary<int, IfcNodeEntity> ifcNodeEntities = new Dictionary<int, IfcNodeEntity>();
            Dictionary<int, IfcPipeEntity> ifcPipeEntities = new Dictionary<int, IfcPipeEntity>();
            Dictionary<int, List<IfcPipeEntity>> ifcPipeToNodeRelations = new Dictionary<int, List<IfcPipeEntity>>();
            
            using StartProject startProject = StartProject.OpenProject(inputFilepath);
            using IFCProject ifcProject = IFCProject.CreateProject("Test");
            
            StartDataArrayItem[] startDataArrayItems = GetArrayData(startProject);
            GroupObjects(
                startDataArrayItems,
                out Dictionary<int, StartAbstractEntity> nodeEntities,
                out Dictionary<int, StartAbstractEntity> pipeEntities,
                out Dictionary<int, StartAbstractEntity> fittingEntities,
                out Dictionary<int, int[]> pipeRelations,
                out Dictionary<int, int> fittingRelations
            );

            foreach (KeyValuePair<int, StartAbstractEntity> nodeEntity in nodeEntities)
            {
                IfcNodeEntity ifcNodeEntity = IfcEntityFactory.CreateEntity<IfcNodeEntity>(nodeEntity.Value);
                ifcNodeEntities.Add(nodeEntity.Key, ifcNodeEntity);
                ifcProject.AddEntity(ifcNodeEntity);
                Console.WriteLine($"Added StartNodeEntity with Id: {nodeEntity.Key}");
            }
            
            foreach (KeyValuePair<int, StartAbstractEntity> pipeEntity in pipeEntities)
            {
                int[] nodeIds = pipeRelations[pipeEntity.Key];
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
                Console.WriteLine($"Added StartPipeEntity with Id: {pipeEntity.Key}");
            }

            foreach (KeyValuePair<int, StartAbstractEntity> fittingEntity in fittingEntities)
            {
                IfcAbstractEntity ifcFittingEntity = IfcEntityFactory.CreateEntity(
                    fittingEntity.Value,
                    ifcNodeEntities[fittingRelations[fittingEntity.Key]],
                    ifcPipeToNodeRelations[fittingRelations[fittingEntity.Key]].ToArray()
                );
                ifcProject.AddEntity(ifcFittingEntity);
                Console.WriteLine($"Added StartFittingEntity with Id: {fittingEntity.Key}");
            }
            
            ifcProject.GroupObjects("Pipe system");
            ifcProject.SaveAs(outputFilepath);
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
            out Dictionary<int, int[]> pipeRelations,
            out Dictionary<int, int> fittingRelations
        )
        {
            nodeEntities = new Dictionary<int, StartAbstractEntity>();
            pipeEntities = new Dictionary<int, StartAbstractEntity>();
            fittingEntities = new Dictionary<int, StartAbstractEntity>();
            pipeRelations = new Dictionary<int, int[]>();
            fittingRelations = new Dictionary<int, int>();

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
                        pipeRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds);
                        break;
                    default:
                        fittingEntities.Add(startDataArrayItem.DataArrayIndex, startAbstractEntity);
                        fittingRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds[0]);
                        break;
                }
            }
        }
    }
}