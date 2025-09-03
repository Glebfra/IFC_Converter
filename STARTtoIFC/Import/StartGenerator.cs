using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.Entities;
using STARTtoIFC.Extensions.Entities.Fittings;
using STARTtoIFC.Extensions.Entities.Segments;
using STARTtoIFC.Importers;
using STARTtoIFC.Tools;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProductExtension;

namespace STARTtoIFC
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
            using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
            {
                IImporter importer = ImporterFactory.CreateImporter(ifcProject.Model, _dataContainer.ImportTypeEnum);
                
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
                
                IfcElement[] ifcPipeSegments = importer.GetPipeSegments(products);
                IfcPipeSegmentEntity[] segmentEntities = importer.CreatePipeSegments(ifcPipeSegments);
                
                IfcElement[] ifcBendPipeFittings = importer.GetBends(products);
                IfcCadBendEntity[] bendEntities = importer.CreateBends(ifcBendPipeFittings, segmentEntities);
                
                IfcElement[] ifcTeeFittings = importer.GetTees(products);
                IfcWeldedTeeEntity[] teeEntities = importer.CreateWeldedTees(ifcTeeFittings, segmentEntities);
                
                using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
                {
                    GeneratePipeEntities(startProject, segmentEntities);
                    GenerateBendEntities(startProject, bendEntities);
                    GenerateTeeEntities(startProject, teeEntities);
                }
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