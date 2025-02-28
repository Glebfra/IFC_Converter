using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using Start;
using Start.API;
using Start.Entities;

namespace STARTtoIFC
{
    internal static class IfcGenerator
    {
        public static void Convert(StartDocument startDocument, string outputFilepath)
        {
            StartDataArrayItem[] startDataArrayItems;
            using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                startDataArrayItems = startProject.GetDataArrayItems()!;
            }

            GroupObjects(
                startDataArrayItems,
                out Dictionary<int, StartAbstractEntity> nodeEntities,
                out Dictionary<int, StartAbstractEntity> pipeEntities,
                out Dictionary<int, StartAbstractEntity> fittingEntities,
                out Dictionary<int, int[]> pipeNodeRelations,
                out Dictionary<int, int> fittingNodeRelations
            );
            
            Dictionary<int, IfcNodeEntity> ifcNodeEntities = new Dictionary<int, IfcNodeEntity>();
            Dictionary<int, IfcPipeEntity> ifcPipeEntities = new Dictionary<int, IfcPipeEntity>();
            Dictionary<int, List<IfcPipeEntity>> ifcPipeToNodeRelations = new Dictionary<int, List<IfcPipeEntity>>();

            foreach (KeyValuePair<int, StartAbstractEntity> nodeEntity in nodeEntities)
            {
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity((StartNodeEntity)nodeEntity.Value);
                ifcNodeEntities.Add(nodeEntity.Key, ifcNodeEntity);
            }

            using (IFCProject ifcProject = IFCProject.CreateProject("StartToIfc"))
            {
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
                }
            
                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilepath);
            }
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
    }
}
