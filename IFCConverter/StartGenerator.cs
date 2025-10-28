using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Fittings.Vertex;
using IFC.Entities.Segments;
using IFCConverter.Extensions.Entities;
using IFCConverter.Extensions.Entities.Fittings;
using IFCConverter.Extensions.Entities.Segments;
using IFCConverter.Importers;
using IFCConverter.Tools;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;

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
        /// <param name="startDocument">StartDocument</param>
        public void Convert(StartDocument startDocument)
        {
            Logger logger = Logger.GetInstance();
            logger.Info("IFCtoSTART importer v." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);

            try
            {
                using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
                {
                    logger.Info($"Opened IFC file: {_dataContainer.InputFilePath}");
                    logger.Info($"IFC schema: {ifcProject.Model.SchemaVersion}");
                    
                    IImporter importer = ImporterFactory.CreateImporter(ifcProject, _dataContainer.ImportTypeEnum);
                    logger.Info($"Using importer: {importer.GetType().Name}");
                    
                    IfcProduct[] products = ifcProject.GetProducts().ToArray();
                    logger.Info($"Found products: {products.Length}");
                    
                    IfcElement[] ifcPipeSegments = importer.GetPipeSegments(products);
                    logger.Info($"Found pipe segments: {ifcPipeSegments.Length}");
                    List<IfcPipeSegmentEntity> segmentEntities = importer.CreatePipeSegments(ifcPipeSegments).ToList();
                    
                    IfcElement[] ifcBendPipeFittings = importer.GetBends(products);
                    logger.Info($"Found bends: {ifcBendPipeFittings.Length}");
                    IfcCadBendEntity[] bendEntities = importer.CreateBends(ifcBendPipeFittings, segmentEntities);
                    
                    IfcElement[] ifcTeeFittings = importer.GetTees(products);
                    logger.Info($"Found tees: {ifcTeeFittings.Length}");
                    IfcWeldedTeeEntity[] teeEntities = importer.CreateWeldedTees(ifcTeeFittings, segmentEntities);
                    
                    IfcElement[] ifcReducerFittings = importer.GetReducers(products);
                    logger.Info($"Found reducers: {ifcReducerFittings.Length}");
                    IfcAbstractReducerEntity[] reducerEntities = importer.CreateReducers(ifcReducerFittings, segmentEntities);
                    
                    using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
                    {
                        GeneratePipeEntities(startProject, segmentEntities);
                        GenerateTeeEntities(startProject, teeEntities);
                        GenerateBendEntities(startProject, bendEntities);
                        GenerateReducerEntities(startProject, reducerEntities);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
            }
        }

        /// <summary>
        /// Generates tee entities and connects them to the corresponding pipe segments and nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="ifcWeldedTeeEntities">IfcWeldedTeeEntities</param>
        private void GenerateTeeEntities(StartProject startProject, IEnumerable<IfcWeldedTeeEntity> ifcWeldedTeeEntities)
        {
            foreach (IfcWeldedTeeEntity ifcWeldedTeeEntity in ifcWeldedTeeEntities)
            {
                IfcPipeSegmentEntity[] pipeSegmentEntities = ifcWeldedTeeEntity.ConnectedEntities
                    .OfType<IfcPipeSegmentEntity>()
                    .ToArray();
                StartObject[] startPipeObjects = pipeSegmentEntities
                    .Select(segment => _startPipeObjects[segment])
                    .ToArray();
                
                StartTeeEntity startTeeEntity = ifcWeldedTeeEntity.ToStartTeeEntity();
                StartObject startTeeObject = GenerateStartEntity(startProject, startTeeEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcWeldedTeeEntity.NodeEntity);
                ConnectNodes(startTeeObject, startNodeObject);
                ConnectObjects(startTeeObject, startPipeObjects);
            }
        }

        /// <summary>
        /// Generates bend entities and connects them to the corresponding pipe segments and nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="ifcCadBendEntities">IfcBendEntities</param>
        private void GenerateBendEntities(StartProject startProject, IEnumerable<IfcCadBendEntity> ifcCadBendEntities)
        {
            foreach (IfcCadBendEntity ifcCadBendEntity in ifcCadBendEntities)
            {
                IfcPipeSegmentEntity[] pipeSegmentEntities = ifcCadBendEntity.ConnectedEntities
                    .OfType<IfcPipeSegmentEntity>()
                    .ToArray();
                StartObject[] startPipeObjects = pipeSegmentEntities
                    .Select(segment => _startPipeObjects[segment])
                    .ToArray();
                
                StartBendEntity startBendEntity = ifcCadBendEntity.ToStartBendEntity();
                StartObject startBendObject = GenerateStartEntity(startProject, startBendEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcCadBendEntity.NodeEntity);
                ConnectNodes(startBendObject, startNodeObject);
                ConnectObjects(startBendObject, startPipeObjects);
            }
        }

        /// <summary>
        /// Generates pipe entities and connects them to their nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="ifcPipeSegmentEntities">IfcPipeSegmentEntities</param>
        private void GeneratePipeEntities(StartProject startProject, IEnumerable<IfcPipeSegmentEntity> ifcPipeSegmentEntities)
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
        /// Generates reducer entities and connects them to the corresponding pipe segments and nodes.
        /// </summary>
        /// <param name="startProject">Start project</param>
        /// <param name="reducerEntities">IfcVertexReducerEccentricEntities</param>
        private void GenerateReducerEntities(StartProject startProject, IEnumerable<IfcAbstractReducerEntity> reducerEntities)
        {
            foreach (IfcAbstractReducerEntity ifcReducerEntity in reducerEntities)
            {
                IfcPipeSegmentEntity[] pipeSegmentEntities = ifcReducerEntity.ConnectedEntities
                    .OfType<IfcPipeSegmentEntity>()
                    .ToArray();
                StartObject[] startPipeObjects = pipeSegmentEntities
                    .Select(segment => _startPipeObjects[segment])
                    .ToArray();
                
                StartReducerEntity startReducerEntity = ifcReducerEntity.ToStartReducerEntity();
                StartObject startReducerObject = GenerateStartEntity(startProject, startReducerEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcReducerEntity.NodeEntity);
                ConnectNodes(startReducerObject, startNodeObject);
                ConnectObjects(startReducerObject, startPipeObjects);
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