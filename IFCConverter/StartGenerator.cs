using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Segments;
using IFCConverter.Extensions.Entities;
using IFCConverter.Importers;
using IFCConverter.Tools;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
using Start.Entities.Segments;
using StartEntityFactory = IFCConverter.Extensions.StartEntityFactory;

namespace IFCConverter
{
    internal class StartGenerator
    {
        private ImportDataContainer _dataContainer;
        private Dictionary<IfcNodeEntity, StartObject> _startNodeObjects;
        private Dictionary<IfcPipeSegmentEntity, StartObject> _startPipeObjects;

        public StartGenerator(ImportDataContainer dataContainer)
        {
            _dataContainer = dataContainer;
            _startNodeObjects = new Dictionary<IfcNodeEntity, StartObject>();
            _startPipeObjects = new Dictionary<IfcPipeSegmentEntity, StartObject>();
        }

        /// <summary>
        /// Main function to convert IFC data to START project.
        /// </summary>
        /// <param name="autoServer">StartAutoServer</param>
        public void Convert(StartAutoServer autoServer)
        {
            Logger logger = Logger.GetInstance();
            logger.Info("IFCtoSTART importer v." + Assembly.GetExecutingAssembly().GetName().Version);

            try
            {
                using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
                {
                    logger.Info($"Opened IFC file: {_dataContainer.InputFilePath}");
                    logger.Info($"IFC schema: {ifcProject.Model.SchemaVersion}");
                    
                    IImporter importer = ImporterFactory.CreateImporter(ifcProject, _dataContainer.ImportTypeEnum);
                    logger.Info($"Using importer: {importer.GetType().Name}");

                    logger.Info($"Searching {nameof(IfcAbstractSegmentEntity)} objects");
                    List<IfcPipeSegmentEntity> pipeSegmentEntities = importer.CreateSegments().ToList();
                    logger.Info($"Found {pipeSegmentEntities.Count} {nameof(IfcAbstractSegmentEntity)} objects");

                    logger.Info($"Searching {nameof(IfcAbstractFittingEntity)} objects");
                    List<IfcAbstractFittingEntity> abstractFittingEntities = importer.CreateFittings(pipeSegmentEntities).ToList();
                    logger.Info($"Found {abstractFittingEntities.Count} {nameof(IfcAbstractFittingEntity)} objects");
                    
                    logger.Info($"Searching {nameof(IfcAbstractAnchorEntity)} objects");
                    List<IfcAbstractAnchorEntity> abstractAnchorEntities = importer.CreateAnchors(pipeSegmentEntities).ToList();
                    logger.Info($"Found {abstractAnchorEntities.Count} {nameof(IfcAbstractAnchorEntity)} objects");

                    using (StartProject startProject = StartProject.OpenFromAutoServer(autoServer))
                    {
                        GenerateSegments(startProject, pipeSegmentEntities);
                        GenerateFittings(startProject, abstractFittingEntities);
                        GenerateAnchors(startProject, abstractAnchorEntities);
                        
                        startProject.OnImportFinish();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Main function to convert IFC data to START project.
        /// </summary>
        /// <param name="startDocument">StartDocument</param>
        public void Convert(StartDocument startDocument)
        {
            Logger logger = Logger.GetInstance();
            logger.Info("IFCtoSTART importer v." + Assembly.GetExecutingAssembly().GetName().Version);

            try
            {
                using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
                {
                    logger.Info($"Opened IFC file: {_dataContainer.InputFilePath}");
                    logger.Info($"IFC schema: {ifcProject.Model.SchemaVersion}");
                    
                    IImporter importer = ImporterFactory.CreateImporter(ifcProject, _dataContainer.ImportTypeEnum);
                    logger.Info($"Using importer: {importer.GetType().Name}");

                    logger.Info($"Searching {nameof(IfcAbstractSegmentEntity)} objects");
                    List<IfcPipeSegmentEntity> pipeSegmentEntities = importer.CreateSegments().ToList();
                    logger.Info($"Found {pipeSegmentEntities.Count} {nameof(IfcAbstractSegmentEntity)} objects");

                    logger.Info($"Searching {nameof(IfcAbstractFittingEntity)} objects");
                    List<IfcAbstractFittingEntity> abstractFittingEntities = importer.CreateFittings(pipeSegmentEntities).ToList();
                    logger.Info($"Found {abstractFittingEntities.Count} {nameof(IfcAbstractFittingEntity)} objects");
                    
                    logger.Info($"Searching {nameof(IfcAbstractAnchorEntity)} objects");
                    List<IfcAbstractAnchorEntity> abstractAnchorEntities = importer.CreateAnchors(pipeSegmentEntities).ToList();
                    logger.Info($"Found {abstractAnchorEntities.Count} {nameof(IfcAbstractAnchorEntity)} objects");

                    using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
                    {
                        GenerateSegments(startProject, pipeSegmentEntities);
                        GenerateFittings(startProject, abstractFittingEntities);
                        GenerateAnchors(startProject, abstractAnchorEntities);
                        
                        startProject.OnImportFinish();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Generates pipe entities and connects them to their nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="ifcPipeSegmentEntities">IfcPipeSegmentEntities</param>
        private void GenerateSegments(StartProject startProject, IEnumerable<IfcPipeSegmentEntity> ifcPipeSegmentEntities)
        {
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in ifcPipeSegmentEntities)
            {
                StartPipeEntity startPipeEntity = ifcPipeSegmentEntity.ToStartPipeEntity();
                StartObject startPipeObject = GenerateStartEntity(startProject, startPipeEntity);
                StartObject[] startNodesObjects = ifcPipeSegmentEntity.NodeEntities
                    .Select(node => GetOrCreateNode(startProject, node))
                    .ToArray();
                ConnectNodes(startPipeObject, startNodesObjects);
                _startPipeObjects.Add(ifcPipeSegmentEntity, startPipeObject);
            }
        }

        /// <summary>
        /// Generates all types of fitting entities and connects them to the corresponding pipe segments and nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="abstractFittingEntities">Abstract fitting entities</param>
        private void GenerateFittings(StartProject startProject, IEnumerable<IfcAbstractFittingEntity> abstractFittingEntities)
        {
            foreach (IfcAbstractFittingEntity ifcAbstractFittingEntity in abstractFittingEntities)
            {
                IfcPipeSegmentEntity[] pipeSegmentEntities = ifcAbstractFittingEntity.ConnectedEntities
                    .OfType<IfcPipeSegmentEntity>()
                    .ToArray();
                StartObject[] startPipeObjects = pipeSegmentEntities
                    .Select(segment => _startPipeObjects[segment])
                    .ToArray();

                StartAbstractEntity startAbstractEntity = StartEntityFactory.CreateEntity(ifcAbstractFittingEntity);
                StartObject startTeeObject = GenerateStartEntity(startProject, startAbstractEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcAbstractFittingEntity.NodeEntity);
                ConnectNodes(startTeeObject, startNodeObject);
                ConnectObjects(startTeeObject, startPipeObjects);
            }
        }
        
        /// <summary>
        /// Generates all types of anchor entities and connects them to the corresponding pipe segments and nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="abstractAnchorEntities">Abstract anchor entities</param>
        private void GenerateAnchors(StartProject startProject, IEnumerable<IfcAbstractAnchorEntity> abstractAnchorEntities)
        {
            foreach (IfcAbstractAnchorEntity ifcAbstractAnchorEntity in abstractAnchorEntities)
            {
                IfcPipeSegmentEntity[] pipeSegmentEntities = ifcAbstractAnchorEntity.ConnectedEntities
                    .OfType<IfcPipeSegmentEntity>()
                    .ToArray();
                StartObject[] startPipeObjects = pipeSegmentEntities
                    .Select(segment => _startPipeObjects[segment])
                    .ToArray();

                StartAbstractEntity startAbstractEntity = StartEntityFactory.CreateEntity(ifcAbstractAnchorEntity);
                StartObject startTeeObject = GenerateStartEntity(startProject, startAbstractEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcAbstractAnchorEntity.NodeEntity);
                ConnectNodes(startTeeObject, startNodeObject);
                ConnectObjects(startTeeObject, startPipeObjects);
            }
        }

        /// <summary>
        /// Get or creates nodes, which contains in Dictionary in START project.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="nodeEntity">IfcNodeEntity</param>
        /// <returns>StartObject</returns>
        private StartObject GetOrCreateNode(StartProject startProject, IfcNodeEntity nodeEntity)
        {
            IfcNodeEntity? nodeEntityKey = _startNodeObjects.Keys.FirstOrDefault(key => key.Equal(nodeEntity));
            if (nodeEntityKey != null)
                nodeEntity = nodeEntityKey;
            bool isCreated = _startNodeObjects.TryGetValue(nodeEntity, out StartObject nodeStartObject);
            if (isCreated) 
                return nodeStartObject;
            
            StartNodeEntity startNodeEntity = nodeEntity.ToStartEntity();
            StartBaseRoot startNode = startProject.AddElement(startNodeEntity.Type, out int index);
            startNode.SetDataJson(0, JsonConvert.SerializeObject(startNodeEntity));
            startNode.SetName(startNodeEntity.Name);
            
            StartObject startNodeObject = new StartObject()
            {
                Index = index,
                Object = startNode
            };
            _startNodeObjects.Add(nodeEntity, startNodeObject);

            return startNodeObject;
        }
        
        /// <summary>
        /// Generates a START entity from an abstract IFC entity and adds it to the START project.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="abstractEntity">StartAbstractEntity</param>
        /// <returns>StartObject</returns>
        private static StartObject GenerateStartEntity(StartProject startProject, StartAbstractEntity abstractEntity)
        {
            string abstractEntityJson = JsonConvert.SerializeObject(abstractEntity);
            StartBaseRoot startObject = startProject.AddElement(abstractEntity.Type, out int index);
            startObject.SetDataJson(0, abstractEntityJson);
            
            return new StartObject()
            {
                Index = index,
                Object = startObject
            };
        }
        
        /// <summary>
        /// Connects a START object to multiple other START objects.
        /// </summary>
        /// <param name="startObject">StartObject</param>
        /// <param name="startObjects">StartObjects</param>
        private static void ConnectObjects(StartObject startObject, params StartObject[] startObjects)
        {
            foreach (StartObject connectObject in startObjects)
            {
                startObject.Object.SetConnElem(connectObject.Index);
            }
        }
        
        /// <summary>
        /// Connects a START object to one or two node START objects.
        /// </summary>
        /// <param name="startObject">StartObject</param>
        /// <param name="startNodeObjects">Node StartObjects</param>
        private static void ConnectNodes(StartObject startObject, params StartObject[] startNodeObjects)
        {
            switch (startNodeObjects.Length)
            {
                case 1:
                    startObject.Object.SetSNode(startNodeObjects[0].Index);
                    break;
                case 2:
                    startObject.Object.SetSNode(startNodeObjects[0].Index);
                    startObject.Object.SetENode(startNodeObjects[1].Index);
                    break;
            }
            ConnectObjects(startObject, startNodeObjects);
        }
    }
}