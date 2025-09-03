using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using IFCConverter.Extensions;
using IFCConverter.Tools;
using Start;
using Start.API;
using Start.Entities;
using Start.Extensions;
using Xbim.Common.Geometry;

namespace IFCConverter
{
    internal class IfcGenerator
    {
        private ExportDataContainer _exportDataContainer;
        private Dictionary<int, IfcNodeEntity> _nodeEntities;
        private Dictionary<int, IfcAbstractSegmentEntity> _twoNodeEntities;

        public IfcGenerator(ExportDataContainer exportDataContainer)
        {
            _exportDataContainer = exportDataContainer;

            _nodeEntities = new Dictionary<int, IfcNodeEntity>();
            _twoNodeEntities = new Dictionary<int, IfcAbstractSegmentEntity>();
        }
        
        public void Convert(StartDocument startDocument)
        {
            Logger logger = Logger.GetInstance();

            using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                StartDataArrayItem[] startDataArrayItems = startProject.GetDataArrayItems()!;
                
                logger.Log($"Found {startDataArrayItems.Length} objects");

                StartDataArrayItem[] nodeItems = startDataArrayItems.GetElementsByType(StartElementType.NODE).ToArray();
                foreach (StartDataArrayItem nodeItem in nodeItems)
                {
                    StartNodeEntity startNodeEntity = (StartNodeEntity)nodeItem.Entity;
                    XbimVector3D nodeCoordinates = new XbimVector3D(
                        startNodeEntity.XCoord.SIProperty,
                        startNodeEntity.YCoord.SIProperty,
                        startNodeEntity.ZCoord.SIProperty
                    );
                    XbimMatrix3D objectMatrix3D = new XbimMatrix3D(nodeCoordinates);
                    IfcNodeEntity ifcNodeEntity = new IfcNodeEntity(objectMatrix3D, startNodeEntity.ID);
                    _nodeEntities.Add(startNodeEntity.ID, ifcNodeEntity);
                    logger.Log($"Added Node with id {startNodeEntity.ID} to IFC.");
                }
            
                using (IFCProject ifcProject = IFCProject.CreateProject(startDocument.GetTitle()))
                {
                    ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.PIPE_ELEMENT, false);
                    ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.CYLINDRICAL_SHELL, false);
                    ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.CONE_ELEMENT, false);
                    ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.RIGID_ELEMENT, true);
                    ConvertTwoNodeObjects(ifcProject, startDataArrayItems, StartElementType.FLEXIBLE_ELEMENT, true);

                    ConvertOneNodeObjects(ifcProject, startDataArrayItems, _exportDataContainer.NumSegments);

                    ifcProject.GroupObjects("Pipe system");
                    ifcProject.SaveAs(_exportDataContainer.OutputFilePath);
                }
            }
        }

        private void ConvertOneNodeObjects(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems,
            int numSegments
        )
        {
            Logger logger = Logger.GetInstance();
            bool isVertex = _exportDataContainer.ExportType == IfcExportTypeEnum.VERTEX;
            
            StartDataArrayItem[] arrayItems = dataArrayItems
                .Where(item => !StartElementTypeExtensions.TwoNodeElementTypes.Contains(item.Type) && item.Type != StartElementType.NODE)
                .ToArray();
            
            foreach (StartDataArrayItem arrayItem in arrayItems)
            {
                try
                {
                    StartDataArrayItem connNode = dataArrayItems
                        .GetConnElements(arrayItem.DataArrayIndex)
                        .GetElementsByType(StartElementType.NODE)
                        .First();
                    StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                        .GetConnElements(arrayItem.DataArrayIndex)
                        .GetElementsByType(StartElementTypeExtensions.TwoNodeElementTypes)
                        .ToArray();

                    IfcNodeEntity ifcNodeEntity = _nodeEntities[connNode.Entity.ID];
                    IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                        .Select(item => _twoNodeEntities[item.DataArrayIndex])
                        .ToArray();

                    IfcAbstractEntity? entity = isVertex
                        ? IfcEntityFactory.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities, numSegments)
                        : IfcEntityFactory.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities);
                    entity ??= IfcEntityFactory.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities, numSegments);
                    entity ??= IfcEntityFactory.CreateEntity(arrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities);

                    if (entity == null)
                    {
                        logger.Error($"Cannot add {arrayItem.Type} with id {arrayItem.DataArrayIndex} to IFC.");
                        continue;
                    }
                    ifcProject.AddEntity(entity);
                    logger.Log($"Added {arrayItem.Type} with id {arrayItem.DataArrayIndex} to IFC.");
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
            }
        }

        private void ConvertTwoNodeObjects(
            IFCProject ifcProject,
            StartDataArrayItem[] dataArrayItems,
            StartElementType type,
            bool useNearEntities
        )
        {
            Logger logger = Logger.GetInstance();

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

                        IfcNodeEntity[] ifcConnNodeEntities = _nodeEntities
                            .Where(pair => nodeIds.Contains(pair.Key))
                            .Select(pair => pair.Value)
                            .ToArray();
                        IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                            .Select(item => _twoNodeEntities.TryGetValue(item.DataArrayIndex, out IfcAbstractSegmentEntity? entity) ? entity : null)
                            .Where(item => item != null)
                            .ToArray()!;

                        if (ifcAbstractSegmentEntities.Length == 0)
                        {
                            index++;
                            continue;
                        }

                        IfcAbstractEntity? ifcObjectEntity = IfcEntityFactory.CreateEntity(objectItem.Entity, ifcConnNodeEntities, ifcAbstractSegmentEntities);
                        if (ifcObjectEntity == null) continue;
                        
                        ifcProject.AddEntity(ifcObjectEntity);
                        _twoNodeEntities.Add(objectItem.DataArrayIndex, (IfcAbstractSegmentEntity)ifcObjectEntity);
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

                        IfcNodeEntity[] ifcConnNodeEntities = _nodeEntities
                            .Where(pair => nodeIds.Contains(pair.Key))
                            .Select(pair => pair.Value)
                            .ToArray();
                        IfcAbstractEntity? ifcObjectEntity = IfcEntityFactory.CreateEntity(objectItem.Entity, ifcConnNodeEntities);
                        if (ifcObjectEntity == null) continue;

                        ifcProject.AddEntity(ifcObjectEntity);
                        _twoNodeEntities.Add(objectItem.DataArrayIndex, (IfcAbstractSegmentEntity)ifcObjectEntity);
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
