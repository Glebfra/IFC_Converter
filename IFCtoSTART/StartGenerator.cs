using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFCtoSTART.Extensions.Entities;
using IFCtoSTART.Importers;
using IFCtoSTART.Tools;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFCtoSTART
{
    internal class StartGenerator
    {
        private DataContainer _dataContainer;
        private Dictionary<IfcNodeEntity, StartObject> _startNodeObjects;

        public StartGenerator(DataContainer dataContainer)
        {
            _dataContainer = dataContainer;
            _startNodeObjects = new Dictionary<IfcNodeEntity, StartObject>();
        }

        public void Convert(StartDocument startDocument)
        {
            IImporter importer = ImporterFactory.CreateImporter(_dataContainer.ImportTypeEnum);
            
            using (IFCProject ifcProject = IFCProject.OpenProject(_dataContainer.InputFilePath))
            {
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
                
                IfcPipeSegment[] ifcPipeSegments = importer.GetPipeSegments(products);
                IfcPipeSegmentEntity[] segmentEntities = importer.CreatePipeSegments(ifcPipeSegments);

                // IfcPipeFitting[] ifcBendPipeFittings = importer.GetBends(products);
                // IfcCadBendEntity[] bendEntities = importer.CreateBends(ifcBendPipeFittings, segmentEntities);

                using (StartProject startProject = StartProject.OpenFromDocument(startDocument))
                {
                    GeneratePipeEntities(startProject, segmentEntities);
                }
            }
        }

        private void GenerateBendEntities(StartProject startProject, IfcCadBendEntity[] ifcCadBendEntities)
        {
            foreach (IfcCadBendEntity ifcCadBendEntity in ifcCadBendEntities)
            {
                StartBendEntity startBendEntity = ifcCadBendEntity.ToStartBendEntity();
                StartObject startBendObject = GenerateStartEntity(startProject, startBendEntity);
                StartObject startNodeObject = GetOrCreateNode(startProject, ifcCadBendEntity.NodeEntity);
                ConnectNodes(startBendObject, startNodeObject);
            }
        }

        private void GeneratePipeEntities(StartProject startProject, IfcPipeSegmentEntity[] ifcPipeSegmentEntities)
        {
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in ifcPipeSegmentEntities)
            {
                StartPipeEntity startPipeEntity = ifcPipeSegmentEntity.ToStartPipeEntity();
                StartObject startPipeObject = GenerateStartEntity(startProject, startPipeEntity);
                StartObject[] startNodesObjects = ifcPipeSegmentEntity.NodeEntities
                    .Select(node => GetOrCreateNode(startProject, node))
                    .ToArray();
                ConnectNodes(startPipeObject, startNodesObjects);
            }
        }

        private StartObject GetOrCreateNode(StartProject startProject, IfcNodeEntity nodeEntity)
        {
            bool isCreated = _startNodeObjects.TryGetValue(nodeEntity, out StartObject nodeStartObject);
            if (isCreated) 
                return nodeStartObject;
            
            StartNodeEntity startNodeEntity = nodeEntity.ToStartEntity();
            StartBaseRoot startNodeObject = startProject.AddElement(startNodeEntity.Type, out int index);
            startNodeObject.SetDataJson(0, JsonConvert.SerializeObject(startNodeEntity));
            
            return new StartObject()
            {
                Index = index,
                Object = startNodeObject
            };
        }
        
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
        
        private static void ConnectObjects(StartObject startObject, params StartObject[] startObjects)
        {
            foreach (StartObject connectObject in startObjects)
            {
                startObject.Object.SetConnElem(connectObject.Index);
            }
        }
        
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