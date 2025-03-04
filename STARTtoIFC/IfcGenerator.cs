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
            GroupedEntities groupedEntities;
            using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                startDataArrayItems = startProject.GetDataArrayItems()!;
                groupedEntities = startProject.GroupEntities(startDataArrayItems);
            }
            Logger.Log($"Successfully grouped objects. Total count is: {startDataArrayItems.Length}");
            
            Dictionary<int, IfcNodeEntity> ifcNodeEntities = new Dictionary<int, IfcNodeEntity>();
            Dictionary<int, IfcPipeEntity> ifcPipeEntities = new Dictionary<int, IfcPipeEntity>();
            Dictionary<int, List<IfcPipeEntity>> ifcPipeToNodeRelations = new Dictionary<int, List<IfcPipeEntity>>();

            foreach (KeyValuePair<int, StartAbstractEntity> nodeEntity in groupedEntities.NodeEntities)
            {
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity((StartNodeEntity)nodeEntity.Value);
                ifcNodeEntities.Add(nodeEntity.Key, ifcNodeEntity);
                Logger.Log($"Added {nodeEntity.Value.GetType().Name} with id {nodeEntity.Key} to IFC.");
            }

            using (IFCProject ifcProject = IFCProject.CreateProject("StartToIfc"))
            {
                foreach (KeyValuePair<int, StartAbstractEntity> pipeEntity in groupedEntities.PipeEntities)
                {
                    int[] nodeIds = groupedEntities.PipeNodeRelations[pipeEntity.Key];
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
                    Logger.Log($"Added {pipeEntity.Value.GetType().Name} with id {pipeEntity.Key} to IFC.");
                }
            
                foreach (KeyValuePair<int, StartAbstractEntity> fittingEntity in groupedEntities.FittingEntities)
                {
                    int fittingId = fittingEntity.Key;
                    StartAbstractEntity fitting = fittingEntity.Value;
                    IfcAbstractEntity ifcFittingEntity = IfcEntityFactory.CreateFittingEntity(
                        fitting,
                        ifcNodeEntities[groupedEntities.FittingNodeRelations[fittingId]],
                        ifcPipeToNodeRelations[groupedEntities.FittingNodeRelations[fittingId]].ToArray()
                    );
                    ifcProject.AddEntity(ifcFittingEntity);
                    Logger.Log($"Added {fittingEntity.Value.GetType().Name} with id {fittingEntity.Key} to IFC.");
                }
            
                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilepath);
            }
        }
    }
}
