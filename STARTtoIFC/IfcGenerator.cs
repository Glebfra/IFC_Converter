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
        public static void Convert(StartDocument startDocument, string outputFilePath)
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
            Dictionary<int, IfcAbstractSegmentEntity> ifcTwoNodeEntities = new Dictionary<int, IfcAbstractSegmentEntity>();
            Dictionary<int, List<IfcAbstractSegmentEntity>> ifcTwoNodeEntitiesRelations = new Dictionary<int, List<IfcAbstractSegmentEntity>>();

            foreach (KeyValuePair<int, StartAbstractEntity> nodeEntity in groupedEntities.NodeEntities)
            {
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity((StartNodeEntity)nodeEntity.Value);
                ifcNodeEntities.Add(nodeEntity.Key, ifcNodeEntity);
                Logger.Log($"Added {nodeEntity.Value.GetType().Name} with id {nodeEntity.Key} to IFC.");
            }

            using (IFCProject ifcProject = IFCProject.CreateProject("StartToIfc"))
            {
                foreach (KeyValuePair<int, StartAbstractEntity> twoNodeEntity in groupedEntities.TwoNodeEntities)
                {
                    int[] nodeIds = groupedEntities.TwoNodeEntitiesRelations[twoNodeEntity.Key];
                    IfcNodeEntity[] ifcConnNodeEntities = nodeIds.Select(nodeId => ifcNodeEntities[nodeId]).ToArray();
                    IfcAbstractSegmentEntity ifcTwoNodeEntity = (IfcAbstractSegmentEntity)IfcEntityFactory.CreateEntity(twoNodeEntity.Value, ifcConnNodeEntities);
                    ifcTwoNodeEntities.Add(twoNodeEntity.Key, ifcTwoNodeEntity);
                
                    foreach (int nodeId in nodeIds)
                    {
                        if (!ifcTwoNodeEntitiesRelations.ContainsKey(nodeId))
                        {
                            ifcTwoNodeEntitiesRelations.Add(nodeId, new List<IfcAbstractSegmentEntity>());
                        }
                        ifcTwoNodeEntitiesRelations[nodeId].Add(ifcTwoNodeEntity);
                    }
                
                    ifcProject.AddEntity(ifcTwoNodeEntity);
                    Logger.Log($"Added {twoNodeEntity.Value.GetType().Name} with id {twoNodeEntity.Key} to IFC.");
                }
            
                foreach (KeyValuePair<int, StartAbstractEntity> oneNodeEntity in groupedEntities.OneNodeEntities)
                {
                    int fittingId = oneNodeEntity.Key;
                    StartAbstractEntity fitting = oneNodeEntity.Value;
                    IfcAbstractEntity ifcFittingEntity = IfcEntityFactory.CreateEntity(
                        fitting,
                        ifcNodeEntities[groupedEntities.OneNodeEntitiesRelations[fittingId]],
                        ifcTwoNodeEntitiesRelations[groupedEntities.OneNodeEntitiesRelations[fittingId]].ToArray()
                    );
                    ifcProject.AddEntity(ifcFittingEntity);
                    Logger.Log($"Added {oneNodeEntity.Value.GetType().Name} with id {oneNodeEntity.Key} to IFC.");
                }
            
                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilePath);
            }
        }
    }
}
