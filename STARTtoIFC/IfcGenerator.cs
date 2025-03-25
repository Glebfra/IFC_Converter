using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Interfaces;
using IFC.Entities.Segments;
using Start;
using Start.API;
using Start.Entities;
using Start.Extensions;
using Xbim.IO.Xml.BsConf;
using EntityCreator = IFC.EntityCreator;

namespace STARTtoIFC
{
    internal static class IfcGenerator
    {
        public static void Convert(StartDocument startDocument, string outputFilePath, IfcExportTypeEnum exportType, int numSegments = 16)
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
                ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.PIPE_ELEMENT, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.CYLINDRICAL_SHELL, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.CONE_ELEMENT, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.RIGID_ELEMENT, nodeEntities, twoNodeEntities, true);
                ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.FLEXIBLE_ELEMENT, nodeEntities, twoNodeEntities, true);

                ConvertOneNodeObjects(ifcProject, startDataArrayItems, nodeEntities, twoNodeEntities, exportType == IfcExportTypeEnum.VERTEX, numSegments);

                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilePath);
            }
        }

        private static void ConvertOneNodeObjects(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems,
            IReadOnlyDictionary<int, IfcNodeEntity> nodeEntities, 
            IReadOnlyDictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
            bool isVertex,
            int numSegments
        )
        {
            Logger logger = Logger.GetInstance();
            EntityCreator entityCreator = new EntityCreator();
            
            StartDataArrayItem[] arrayItems = dataArrayItems
                .Where(item => !StartElementTypeExtensions.TwoNodeElementTypes.Contains(item.Type) && item.Type != StartElementType.NODE)
                .ToArray();
            
            foreach (StartDataArrayItem arrayItem in arrayItems)
            {
                StartDataArrayItem connNode = dataArrayItems
                    .GetConnElements(arrayItem.DataArrayIndex)
                    .GetElementsByType(StartElementType.NODE)
                    .First();
                StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                    .GetConnElements(arrayItem.DataArrayIndex)
                    .GetElementsByType(StartElementTypeExtensions.TwoNodeElementTypes)
                    .ToArray();

                IfcNodeEntity ifcNodeEntity = nodeEntities[connNode.Entity.ID];
                IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                    .Select(item => twoNodeEntities[item.DataArrayIndex])
                    .ToArray();
                
                IfcAbstractEntity? entity = isVertex 
                    ? entityCreator.CreateVertexEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities, numSegments) 
                    : entityCreator.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities);
                entity ??= entityCreator.CreateVertexEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities, numSegments);
                entity ??= entityCreator.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities);
                
                if (entity == null)
                {
                    logger.Error($"Cannot add {arrayItem.Type} with id {arrayItem.DataArrayIndex} to IFC.");
                    continue;
                }
                ifcProject.AddEntity(entity);
                logger.Log($"Added {arrayItem.Type} with id {arrayItem.DataArrayIndex} to IFC.");
            }
        }

        private static void ConvertTwoNodeObjects(
            IFCProject ifcProject,
            StartDataArrayItem[] dataArrayItems,
            StartElementType type,
            Dictionary<int, IfcNodeEntity> nodeEntities,
            Dictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
            bool useNearEntities = false
        )
        {
            Logger logger = Logger.GetInstance();
            EntityCreator entityCreator = new EntityCreator();
            
            StartDataArrayItem[] objectItems = dataArrayItems.GetElementsByType(type).ToArray();
            if (useNearEntities)
            {
                List<StartDataArrayItem> unconvertedObjects = objectItems.ToList();
                int index = 0;
                while (unconvertedObjects.Count != 0)
                {
                    try
                    {
                        if (index >= unconvertedObjects.Count) return;

                        StartDataArrayItem objectItem = unconvertedObjects[index];

                        StartDataArrayItem[] connNodes = dataArrayItems
                            .GetConnElements(objectItem.DataArrayIndex)
                            .GetElementsByType(StartElementType.NODE)
                            .ToArray();
                        StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                            .GetConnElements(objectItem.DataArrayIndex)
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
                            .Select(item => twoNodeEntities.TryGetValue(item.DataArrayIndex, out IfcAbstractSegmentEntity? entity) ? entity : null)
                            .Where(item => item != null)
                            .ToArray()!;

                        if (ifcAbstractSegmentEntities.Length == 0)
                        {
                            index++;
                            continue;
                        }

                        IfcAbstractEntity? ifcObjectEntity = entityCreator.CreateEntity(objectItem.Entity, ifcConnNodeEntities, ifcAbstractSegmentEntities);
                        if (ifcObjectEntity == null) continue;
                        
                        ifcProject.AddEntity(ifcObjectEntity);
                        twoNodeEntities.Add(objectItem.DataArrayIndex, (IfcAbstractSegmentEntity)ifcObjectEntity);
                        logger.Log($"Added {objectItem.Type} with id {objectItem.DataArrayIndex} to IFC.");
                        unconvertedObjects.Remove(objectItem);
                        index = 0;
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.ToString());
                        index++;
                    }
                }
            }
            else
            {
                foreach (StartDataArrayItem objectItem in objectItems)
                {
                    try
                    {
                        StartDataArrayItem[] connNodes = dataArrayItems
                            .GetConnElements(objectItem.DataArrayIndex)
                            .GetElementsByType(StartElementType.NODE)
                            .ToArray();

                        int[] nodeIds = connNodes
                            .Select(node => node.NodeIds[0])
                            .ToArray();

                        IfcNodeEntity[] ifcConnNodeEntities = nodeEntities
                            .Where(pair => nodeIds.Contains(pair.Key))
                            .Select(pair => pair.Value)
                            .ToArray();
                        IfcAbstractEntity? ifcObjectEntity = entityCreator.CreateEntity(objectItem.Entity, ifcConnNodeEntities);
                        if (ifcObjectEntity == null) continue;

                        ifcProject.AddEntity(ifcObjectEntity);
                        twoNodeEntities.Add(objectItem.DataArrayIndex, (IfcAbstractSegmentEntity)ifcObjectEntity);
                        logger.Log($"Added {objectItem.Type} with id {objectItem.DataArrayIndex} to IFC.");
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.ToString());
                    }
                }
            }
        }
    }
}
