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
                ConvertTwoNodeObjects<StartPipeEntity, IfcPipeEntity>(ifcProject, startDataArrayItems, StartElementType.PIPE_ELEMENT, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects<StartPipeEntity, IfcCylindricalShellEntity>(ifcProject, startDataArrayItems, StartElementType.CYLINDRICAL_SHELL, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects<StartConeElementEntity, IfcConeElementEntity>(ifcProject, startDataArrayItems, StartElementType.CONE_ELEMENT, nodeEntities, twoNodeEntities);
                ConvertTwoNodeObjects<StartRigidElementEntity, IfcRigidElementEntity>(ifcProject, startDataArrayItems, StartElementType.RIGID_ELEMENT, nodeEntities, twoNodeEntities, true);
                ConvertTwoNodeObjects<StartFlexibleElementEntity, IfcFlexibleSegmentEntity>(ifcProject, startDataArrayItems, StartElementType.FLEXIBLE_ELEMENT, nodeEntities, twoNodeEntities, true);

                if (exportType == IfcExportTypeEnum.VERTEX)
                {
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.ELBOW, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.PIPE_BEND, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.MILTER_BEND, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.WELDED_BEND, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.LONG_RADIUS_PIPE_BEND, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.PRE_STRESSED_PIPE_BEND, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartBendEntity, IfcVertexBendEntity>(ifcProject, startDataArrayItems, StartElementType.SADDLE_BEND, nodeEntities, twoNodeEntities, numSegments);

                    ConvertOneNodeObjects<StartAxialExpansionJointEntity, IfcVertexAxialExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.AXIAL_EXPANSION_JOINT, nodeEntities, twoNodeEntities, numSegments);
                    ConvertOneNodeObjects<StartAxialExpansionJointEntity, IfcVertexAxialExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.AXIAL_EXPANSION_SLIP_JOINT, nodeEntities, twoNodeEntities, numSegments);
                }
                else
                {
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.ELBOW, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.PIPE_BEND, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.MILTER_BEND, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.WELDED_BEND, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.LONG_RADIUS_PIPE_BEND, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.PRE_STRESSED_PIPE_BEND, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartBendEntity, IfcBendEntity>(ifcProject, startDataArrayItems, StartElementType.SADDLE_BEND, nodeEntities, twoNodeEntities);
                    
                    ConvertOneNodeObjects<StartAxialExpansionJointEntity, IfcAxialExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.AXIAL_EXPANSION_JOINT, nodeEntities, twoNodeEntities);
                    ConvertOneNodeObjects<StartAxialExpansionJointEntity, IfcAxialExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.AXIAL_EXPANSION_SLIP_JOINT, nodeEntities, twoNodeEntities);
                }
                
                ConvertOneNodeObjects<StartBendEntity, IfcMilterJointEntity>(ifcProject, startDataArrayItems, StartElementType.MILTER_JOINT, nodeEntities, twoNodeEntities);

                ConvertOneNodeObjects<StartTeeEntity, IfcWeldedTeeEntity>(ifcProject, startDataArrayItems, StartElementType.WELDED_TEE, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcWeldoletEntity>(ifcProject, startDataArrayItems, StartElementType.WELDOLET, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcSweepoletEntity>(ifcProject, startDataArrayItems, StartElementType.SWEEPOLET, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcFabricatedTeeEntity>(ifcProject, startDataArrayItems, StartElementType.FABRICATED_TEE, nodeEntities, twoNodeEntities);
                ConvertOneNodeObjects<StartTeeEntity, IfcStubInEntity>(ifcProject, startDataArrayItems, StartElementType.STUB_IN, nodeEntities, twoNodeEntities);

                ConvertOneNodeObjects<StartReducerEntity, IfcVertexReducerConcentricEntity>(ifcProject, startDataArrayItems, StartElementType.REDUCER_CONCENTRIC, nodeEntities, twoNodeEntities, numSegments);
                ConvertOneNodeObjects<StartReducerEntity, IfcVertexReducerEccentricEntity>(ifcProject, startDataArrayItems, StartElementType.REDUCER_ECCENTRIC, nodeEntities, twoNodeEntities, numSegments);

                ConvertOneNodeObjects<StartArmatureEntity, IfcVertexValveEntity>(ifcProject, startDataArrayItems, StartElementType.VALVE, nodeEntities, twoNodeEntities, numSegments);
                ConvertOneNodeObjects<StartArmatureEntity, IfcVertexFlangeEntity>(ifcProject, startDataArrayItems, StartElementType.FLANGE, nodeEntities, twoNodeEntities, numSegments);
                
                ConvertOneNodeObjects<StartAngularExpansionJointEntity, IfcVertexAngularExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.GIMBAL_EXPANSION_JOINT, nodeEntities, twoNodeEntities, numSegments);
                ConvertOneNodeObjects<StartLateralExpansionJointEntity, IfcVertexLateralExpansionJointEntity>(ifcProject, startDataArrayItems, StartElementType.LATERAL_EXPANSION_JOINT, nodeEntities, twoNodeEntities, numSegments);

                ifcProject.GroupObjects("Pipe system");
                ifcProject.SaveAs(outputFilePath);
            }
        }

        private static void ConvertOneNodeObjects(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems,
            IReadOnlyDictionary<int, IfcNodeEntity> nodeEntities, 
            IReadOnlyDictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
            int numSegments
        )
        {
            Logger logger = Logger.GetInstance();
            EntityCreator entityCreator = new EntityCreator();
            
            foreach (StartDataArrayItem startDataArrayItem in dataArrayItems)
            {
                StartDataArrayItem connNode = dataArrayItems
                    .GetConnElements(startDataArrayItem.DataArrayIndex)
                    .GetElementsByType(StartElementType.NODE)
                    .First();
                StartDataArrayItem[] connTwoNodesElements = dataArrayItems
                    .GetConnElements(startDataArrayItem.DataArrayIndex)
                    .GetElementsByType(StartElementTypeExtensions.TwoNodeElementTypes)
                    .ToArray();

                IfcNodeEntity ifcNodeEntity = nodeEntities[connNode.Entity.ID];
                IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                    .Select(item => twoNodeEntities[item.DataArrayIndex])
                    .ToArray();
                
                IfcAbstractEntity? entity = entityCreator.CreateEntity(startDataArrayItem.Entity, ifcNodeEntity, ifcAbstractSegmentEntities);
                if (entity == null)
                {
                    logger.Log($"Cannot add {startDataArrayItem.Type} with id {startDataArrayItem.DataArrayIndex} to IFC.");
                    continue;
                }
                ifcProject.AddEntity(entity);
                logger.Log($"Added {startDataArrayItem.Type} with id {startDataArrayItem.DataArrayIndex} to IFC.");
            }
        }

        private static void ConvertTwoNodeObjects<T, U>(
            IFCProject ifcProject,
            StartDataArrayItem[] dataArrayItems,
            StartElementType type,
            Dictionary<int, IfcNodeEntity> nodeEntities,
            Dictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
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
                    try
                    {
                        if (index >= unconvertedObjects.Count) return;

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
                            .Select(item => twoNodeEntities.TryGetValue(item.DataArrayIndex, out IfcAbstractSegmentEntity? entity) ? entity : null)
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
                        T startObjectEntity = (T)objectItem.Entity;
                        StartDataArrayItem[] connNodes = dataArrayItems
                            .GetConnElements(startObjectEntity.ID)
                            .GetElementsByType(StartElementType.NODE)
                            .ToArray();

                        int[] nodeIds = connNodes
                            .Select(node => node.NodeIds[0])
                            .ToArray();

                        IfcNodeEntity[] ifcConnNodeEntities = nodeEntities
                            .Where(pair => nodeIds.Contains(pair.Key))
                            .Select(pair => pair.Value)
                            .ToArray();
                        U ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcConnNodeEntities);

                        ifcProject.AddEntity(ifcObjectEntity);
                        twoNodeEntities.Add(startObjectEntity.ID, ifcObjectEntity);
                        logger.Log($"Added {startObjectEntity.Type} with id {startObjectEntity.ID} to IFC.");
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.ToString());
                    }
                }
            }
        }

        private static void ConvertOneNodeObjects<T, U>(
            IFCProject ifcProject, 
            StartDataArrayItem[] dataArrayItems, 
            StartElementType type, 
            IReadOnlyDictionary<int, IfcNodeEntity> nodeEntities, 
            IReadOnlyDictionary<int, IfcAbstractSegmentEntity> twoNodeEntities,
            params object[] args
        )
            where T : StartAbstractEntity
            where U : IIfcOneNodeEntity
        {
            Logger logger = Logger.GetInstance();
            StartDataArrayItem[] objectItems = dataArrayItems.GetElementsByType(type).ToArray();
            foreach (StartDataArrayItem objectItem in objectItems)
            {
                try
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
                    IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities = connTwoNodesElements
                        .Select(item => twoNodeEntities[item.DataArrayIndex])
                        .ToArray();

                    U ifcObjectEntity;
                    if (args.Length != 0)
                    {
                        ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcNodeEntity, ifcAbstractSegmentEntities, args);
                    }
                    else
                    {
                        ifcObjectEntity = (U)Activator.CreateInstance(typeof(U), startObjectEntity, ifcNodeEntity, ifcAbstractSegmentEntities);
                    }
                    
                    ifcProject.AddEntity(ifcObjectEntity);
                    logger.Log($"Added {startObjectEntity.Type} with id {startObjectEntity.ID} to IFC.");
                }
                catch (Exception e)
                {
                    logger.Error(e.ToString());
                }
            }
        }
    }
}
