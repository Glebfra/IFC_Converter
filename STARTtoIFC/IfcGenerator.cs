using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using IFC.Entities.Interfaces;
using IFC.Entities.Segments;
using Start;
using Start.API;
using Start.Entities;
using Start.Extensions;

namespace STARTtoIFC
{
    internal static class IfcGenerator
    {
        public static void Convert(StartDocument startDocument, string outputFilePath)
        {
            Logger logger = Logger.GetInstance();
            
            Dictionary<int, IfcNodeEntity> nodeEntities = new Dictionary<int, IfcNodeEntity>();
            Dictionary<int, IfcAbstractSegmentEntity> twoNodeEntities = new Dictionary<int, IfcAbstractSegmentEntity>();
            
            StartDataArrayItem[] startDataArrayItems;
            using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                startDataArrayItems = startProject.GetDataArrayItems()!;
            }
            logger.Log($"Found {startDataArrayItems.Length} objects");

            StartDataArrayItem[] nodeItems = startDataArrayItems.GetElementsByType(StartElementType.NODE).ToArray();
            foreach (StartDataArrayItem nodeItem in nodeItems)
            {
                StartNodeEntity startNodeEntity = (StartNodeEntity)nodeItem.Entity;
                IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(startNodeEntity);
                nodeEntities.Add(startNodeEntity.ID, ifcNodeEntity);
                logger.Log($"Added Node with id {startNodeEntity.ID} to IFC.");
            }

            using (IFCProject ifcProject = IFCProject.CreateProject("IFC"))
            {
                ConvertTwoNodeObjects<StartPipeEntity, IfcPipeEntity>(ifcProject, startDataArrayItems, StartElementType.PIPE_ELEMENT, nodeEntities, ref twoNodeEntities);
                ConvertTwoNodeObjects<StartPipeEntity, IfcCylindricalShellEntity>(ifcProject, startDataArrayItems, StartElementType.CYLINDRICAL_SHELL, nodeEntities, ref twoNodeEntities);
                ConvertTwoNodeObjects<StartRigidElementEntity, IfcRigidElementEntity>(ifcProject, startDataArrayItems, StartElementType.RIGID_ELEMENT, nodeEntities, ref twoNodeEntities, true);
                ConvertTwoNodeObjects<StartFlexibleElementEntity, IfcFlexibleSegmentEntity>(ifcProject, startDataArrayItems, StartElementType.FLEXIBLE_ELEMENT, nodeEntities, ref twoNodeEntities, true);

                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.ELBOW, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.PIPE_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.MILTER_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.WELDED_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.LONG_RADIUS_PIPE_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.PRE_STRESSED_PIPE_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.SADDLE_BEND, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartBendEntity, IfcMilterJointEntity>(ifcProject, startDataArrayItems, StartElementType.MILTER_JOINT, nodeEntities, twoNodeEntities);

                ConvertOneNodeObjects<StartTeeEntity, IfcWeldedTeeEntity>(ifcProject, startDataArrayItems, StartElementType.WELDED_TEE, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcWeldoletEntity>(ifcProject, startDataArrayItems, StartElementType.WELDOLET, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcSweepoletEntity>(ifcProject, startDataArrayItems, StartElementType.SWEEPOLET, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcFabricatedTeeEntity>(ifcProject, startDataArrayItems, StartElementType.FABRICATED_TEE, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcStubInEntity>(ifcProject, startDataArrayItems, StartElementType.STUB_IN, nodeEntities, twoNodeEntities);

                ConvertOneNodeObjects<StartReducerEntity, IfcReducerConcentricEntity>(ifcProject, startDataArrayItems, StartElementType.REDUCER_CONCENTRIC, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartReducerEntity, IfcReducerEccentricEntity>(ifcProject, startDataArrayItems, StartElementType.REDUCER_ECCENTRIC, nodeEntities, twoNodeEntities);

                ConvertOneNodeObjects<StartArmatureEntity, IfcValveEntity>(ifcProject, startDataArrayItems, StartElementType.VALVE, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartArmatureEntity, IfcFlangeEntity>(ifcProject, startDataArrayItems, StartElementType.FLANGE, nodeEntities, twoNodeEntities);

                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilePath);
            }
        }

        private static void ConvertTwoNodeObjects<T, U>(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems, 
            StartElementType type, 
            Dictionary<int, IfcNodeEntity> nodeEntities, 
            ref Dictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
            bool useNearEntities = false
        )
            where T : StartAbstractEntity
            where U : IfcAbstractSegmentEntity
        {
            Logger logger = Logger.GetInstance();
            StartDataArrayItem[] objectItems = dataArrayItems.GetElementsByType(type).ToArray();
            if (useNearEntities)
            {
                List<StartDataArrayItem> unconvertedObjects = objectItems.ToList();
                int index = 0;
                while (unconvertedObjects.Count != 0)
                {
                    if (index >= unconvertedObjects.Count) return;
                    
                    Dictionary<int, IfcAbstractSegmentEntity> entities = twoNodeEntities;
                    StartDataArrayItem objectItem = unconvertedObjects[index];
                    T startObjectEntity = (T)objectItem.Entity;
                    
                    StartDataArrayItem[] connNodes = dataArrayItems
                        .GetConnElements(startObjectEntity.ID)
                        .GetElementsByType(StartElementType.NODE)
                        .ToArray();
                    StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                        .GetConnElements(startObjectEntity.ID)
                        .GetElementsByType(StartElementTypeExtensions.TwoNodeElementTypes)
                        .ToArray();
                    int[] nodeIds = connNodes
                        .Select(node => node.NodeIds[0])
                        .ToArray();
                    
                    IfcNodeEntity[] ifcConnNodeEntities = nodeEntities
                        .Where(pair => nodeIds.Contains(pair.Key))
                        .Select(pair => pair.Value)
                        .ToArray();
                    IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                        .Select(item => entities.TryGetValue(item.DataArrayIndex, out IfcAbstractSegmentEntity? entity) ? entity : null)
                        .Where(item => item != null)
                        .ToArray()!;
                    
                    if (ifcAbstractSegmentEntities.Length == 0)
                    {
                        index++;
                        continue;
                    }
                    U ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcConnNodeEntities, ifcAbstractSegmentEntities);
                    ifcProject.AddEntity(ifcObjectEntity);
                    twoNodeEntities.Add(startObjectEntity.ID, ifcObjectEntity);
                    logger.Log($"Added {startObjectEntity.Type} with id {startObjectEntity.ID} to IFC.");
                    
                    unconvertedObjects.Remove(objectItem);
                    index = 0;
                }
            }
            else
            {
                foreach (StartDataArrayItem objectItem in objectItems)
                {
                    T startObjectEntity = (T)objectItem.Entity;
                    StartDataArrayItem[] connNodes = dataArrayItems
                        .GetConnElements(startObjectEntity.ID)
                        .GetElementsByType(StartElementType.NODE)
                        .ToArray();
                    
                    int[] nodeIds = connNodes
                        .Select(node => node.NodeIds[0])
                        .ToArray();
                
                    IfcNodeEntity[] ifcConnNodeEntities = nodeEntities.Where(pair => nodeIds.Contains(pair.Key)).Select(pair => pair.Value).ToArray();
                    U ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcConnNodeEntities);
                
                    ifcProject.AddEntity(ifcObjectEntity);
                    twoNodeEntities.Add(startObjectEntity.ID, ifcObjectEntity);
                    logger.Log($"Added {startObjectEntity.Type} with id {startObjectEntity.ID} to IFC.");
                }
            }
        }

        private static void ConvertOneNodeObjects<T, U>(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems, 
            StartElementType type, 
            IReadOnlyDictionary<int, IfcNodeEntity> nodeEntities, 
            IReadOnlyDictionary<int, IfcAbstractSegmentEntity> twoNodeEntities
        )
            where T : StartAbstractEntity
            where U : IIfcOneNodeEntity
        {
            Logger logger = Logger.GetInstance();
            StartDataArrayItem[] objectItems = dataArrayItems.GetElementsByType(type).ToArray();
            foreach (StartDataArrayItem objectItem in objectItems)
            {
                T startObjectEntity = (T)objectItem.Entity;
                StartDataArrayItem connNode = dataArrayItems
                    .GetConnElements(startObjectEntity.ID)
                    .GetElementsByType(StartElementType.NODE)
                    .First();
                StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                    .GetConnElements(startObjectEntity.ID)
                    .GetElementsByType(StartElementTypeExtensions.TwoNodeElementTypes)
                    .ToArray();

                IfcNodeEntity ifcNodeEntity = nodeEntities[connNode.Entity.ID];
                IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements.Select(item => twoNodeEntities[item.DataArrayIndex]).ToArray();

                U ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcNodeEntity, ifcAbstractSegmentEntities);
                ifcProject.AddEntity(ifcObjectEntity);
                logger.Log($"Added {startObjectEntity.Type} with id {startObjectEntity.ID} to IFC.");
            }
        }
    }
}
