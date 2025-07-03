using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFC.Extensions;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities.Fittings;
using Start.Entities.Segments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IfcConverter
{
    internal class IfcConverter : IDisposable
    {
        private int _nodeIndex = 1;
        
        private Dictionary<IfcNodeEntity, StartObject> _nodeEntities = new Dictionary<IfcNodeEntity, StartObject>();
        private Dictionary<IfcWeldedTeeEntity, StartObject> _weldedTees = new Dictionary<IfcWeldedTeeEntity, StartObject>();
        private Dictionary<IfcPipeSegmentEntity, StartObject> _pipeEntities = new Dictionary<IfcPipeSegmentEntity, StartObject>();

        public void Import(string filePath, string ctpFilePath)
        {
            using (IFCProject ifcProject = IFCProject.OpenProject(filePath))
            {
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
                IfcPipeSegment[] pipeSegments = products.OfType<IfcPipeSegment>().ToArray();
                IfcPipeSegmentEntity[] pipeSegmentEntities = CreatePipeSegmentEntities(pipeSegments);

                IfcPipeFitting[] pipeFittings = products.OfType<IfcPipeFitting>().ToArray();
                IfcPipeFitting[] junctions = FilterJunctions(pipeFittings);
                IfcWeldedTeeEntity[] weldedTeeEntities = CreateWeldedTeeEntities(junctions, pipeSegmentEntities);
                
                using (StartProject startProject = StartProject.OpenProject(ctpFilePath))
                {
                    GenerateWeldedTees(startProject, weldedTeeEntities);
                    GeneratePipeSegments(startProject, pipeSegmentEntities);
                }
            }
        }

        public void Dispose()
        {
            foreach (KeyValuePair<IfcNodeEntity, StartObject> kvp in _nodeEntities)
            {
                kvp.Value.Object.Dispose();
            }
            foreach (KeyValuePair<IfcPipeSegmentEntity, StartObject> kvp in _pipeEntities)
            {
                kvp.Value.Object.Dispose();
            }
            foreach (KeyValuePair<IfcWeldedTeeEntity,StartObject> kvp in _weldedTees)
            {
                kvp.Value.Object.Dispose();
            }
            _nodeEntities = null;
            _pipeEntities = null;
            _weldedTees = null;
        }

        private void GenerateWeldedTees(StartProject startProject, IfcWeldedTeeEntity[] weldedTeeEntities)
        {
            foreach (IfcWeldedTeeEntity ifcWeldedTeeEntity in weldedTeeEntities)
            {
                StartTeeEntity startTeeEntity = (StartTeeEntity)ifcWeldedTeeEntity.StartAbstractEntity;
                string startTeeJson = JsonConvert.SerializeObject(startTeeEntity);

                StartBaseRoot startTeeObject = startProject.AddElement(StartElementType.WELDED_TEE, out int teeIndex);
                
                startTeeObject.SetDataJson(0, startTeeJson);
                _weldedTees.Add(ifcWeldedTeeEntity, new StartObject() {Object = startTeeObject, Index = teeIndex});

                IfcNodeEntity ifcNodeEntity = ifcWeldedTeeEntity.NodeEntity;
                StartObject nodeObject = GetOrCreateNode(startProject, ifcNodeEntity);
                
                int connNodeIndex = nodeObject.Index;
                startTeeObject.SetSNode(connNodeIndex);
                startTeeObject.SetConnElem(connNodeIndex);

                foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in ifcWeldedTeeEntity.AbstractSegmentEntities)
                {
                    XbimVector3D displacement = ifcAbstractSegmentEntity.ReplaceNearestNode(ifcNodeEntity);
                }
            }
        }

        private void GeneratePipeSegments(StartProject startProject, IfcPipeSegmentEntity[] pipeSegmentEntities)
        {
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in pipeSegmentEntities)
            {
                StartPipeEntity startPipeEntity = (StartPipeEntity)ifcPipeSegmentEntity.StartAbstractEntity;
                string startPipeJson = JsonConvert.SerializeObject(startPipeEntity);

                StartBaseRoot startPipeObject = startProject.AddElement(StartElementType.PIPE_ELEMENT, out int pipeIndex);
                startPipeObject.SetDataJson(0, startPipeJson);
                _pipeEntities.Add(ifcPipeSegmentEntity, new StartObject() {Object = startPipeObject, Index = pipeIndex});

                StartObject[] nodeEntityObjects = new StartObject[2];
                for (int i = 0; i < 2; i++)
                {
                    IfcNodeEntity ifcNodeEntity = ifcPipeSegmentEntity.NodeEntities[i];
                    nodeEntityObjects[i] = GetOrCreateNode(startProject, ifcNodeEntity);
                }

                int[] connNodeIndexes = nodeEntityObjects.Select(item => item.Index).ToArray();
                startPipeObject.SetSNode(connNodeIndexes[0]);
                startPipeObject.SetENode(connNodeIndexes[1]);
                startPipeObject.SetConnElem(connNodeIndexes[0]);
                startPipeObject.SetConnElem(connNodeIndexes[1]);
            }
        }

        private StartObject GetOrCreateNode(StartProject startProject, IfcNodeEntity ifcNodeEntity)
        {
            if (_nodeEntities.TryGetValue(ifcNodeEntity, out StartObject node)) 
                return node;
            
            string startNodeJson = JsonConvert.SerializeObject(ifcNodeEntity.NodeEntity);
            StartBaseRoot nodeEntityObject = startProject.AddElement(StartElementType.NODE, out int nodeIndex);
            nodeEntityObject.SetDataJson(0, startNodeJson);
            nodeEntityObject.SetName((_nodeIndex++).ToString());
            _nodeEntities.Add(ifcNodeEntity, new StartObject() {Object = nodeEntityObject, Index = nodeIndex});

            return _nodeEntities[ifcNodeEntity];
        }

        private static IfcPipeFitting[] FilterJunctions(IEnumerable<IfcPipeFitting> pipeFittings)
        {
            return pipeFittings
                .Where(item => item.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                .ToArray();
        }

        private static IfcWeldedTeeEntity[] CreateWeldedTeeEntities(IEnumerable<IfcPipeFitting> pipeFittings, IfcAbstractSegmentEntity[] segmentEntities)
        {
            return pipeFittings
                .Select(item => IfcWeldedTeeEntityExtensions.CreateFromIfc(item, segmentEntities))
                .ToArray();
        }

        private static IfcPipeSegmentEntity[] CreatePipeSegmentEntities(IEnumerable<IfcPipeSegment> pipeSegments)
        {
            return pipeSegments
                .Select(IfcPipeSegmentExtensions.CreateFromIfc)
                .ToArray();
        }
    }
}