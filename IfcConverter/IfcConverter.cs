using System;
using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using Newtonsoft.Json;
using Start;
using Start.API;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IfcConverter
{
    internal class IfcConverter : IDisposable
    {
        #if NEW

        public void Import(string filePath, string ctpFilePath)
        {
            
        }
        
        public void Dispose()
        {
            
        }
        
        #else
        
        private int _nodeIndex = 1;
        
        private Dictionary<IfcNodeEntity, StartObject> _nodeEntities = new Dictionary<IfcNodeEntity, StartObject>();
        private Dictionary<IfcPipeSegmentEntity, StartObject> _pipeEntities = new Dictionary<IfcPipeSegmentEntity, StartObject>();

        public void Import(string filePath, string ctpFilePath)
        {
            using (IFCProject ifcProject = IFCProject.OpenProject(filePath))
            {
                IfcProduct[] products = ifcProject.GetProducts().ToArray();
                IfcPipeSegment[] pipeSegments = products.OfType<IfcPipeSegment>().ToArray();
                IfcPipeSegmentEntity[] pipeSegmentEntities = CreatePipeSegmentEntities(pipeSegments);

                IfcPipeFitting[] pipeFittings = products.OfType<IfcPipeFitting>().ToArray();

                IfcPipeFitting[] tees = FilterTees(pipeFittings);
                IfcWeldedTeeEntity[] weldedTeeEntities = CreateWeldedTeeEntities(tees, pipeSegmentEntities);

                IfcPipeFitting[] bends = FilterBends(pipeFittings);
                IfcCadBendEntity[] bendEntities = CreateBendEntities(bends, pipeSegmentEntities);

                using (StartProject startProject = StartProject.OpenProject(ctpFilePath))
                {
                    GenerateFitting(startProject, weldedTeeEntities);
                    GenerateFitting(startProject, bendEntities);
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
            _nodeEntities = null;
            _pipeEntities = null;
        }

        private void GenerateFitting(StartProject startProject, IEnumerable<IfcAbstractFittingEntity> fittings)
        {
            foreach (IfcAbstractFittingEntity fitting in fittings)
            {
                string startEntityJson = JsonConvert.SerializeObject(fitting.StartAbstractEntity);

                StartBaseRoot startEntityObject = startProject.AddElement(fitting.Type, out int entityIndex);
                startEntityObject.SetDataJson(0, startEntityJson);

                IfcNodeEntity ifcNodeEntity = fitting.NodeEntity;
                StartObject nodeObject = GetOrCreateNode(startProject, ifcNodeEntity);

                int connNodeIndex = nodeObject.Index;
                startEntityObject.SetSNode(connNodeIndex);
                startEntityObject.SetConnElem(connNodeIndex);
                
                foreach (IfcAbstractSegmentEntity abstractSegmentEntity in fitting.AbstractSegmentEntities)
                {
                    if (abstractSegmentEntity is IfcPipeSegmentEntity pipeSegmentEntity)
                    {
                        pipeSegmentEntity.ReplaceNearestNodeAndRescale(ifcNodeEntity);
                    }
                }
            }
        }

        private void GeneratePipeSegments(StartProject startProject, IfcPipeSegmentEntity[] pipeSegmentEntities)
        {
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in pipeSegmentEntities)
            {
                string startPipeJson = JsonConvert.SerializeObject(ifcPipeSegmentEntity.StartAbstractEntity);

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

        private static IfcPipeFitting[] FilterTees(IfcPipeFitting[] pipeFittings)
        {
            List<IfcPipeFitting> tees = new List<IfcPipeFitting>();
            foreach (IfcPipeFitting pipeFitting in pipeFittings)
            {
                if (pipeFitting.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                {
                    tees.Add(pipeFitting);
                    continue;
                }
            }

            return tees.ToArray();
        }

        private static IfcPipeFitting[] FilterBends(IfcPipeFitting[] pipeFittings)
        {
            List<IfcPipeFitting> bends = new List<IfcPipeFitting>();
            foreach (IfcPipeFitting pipeFitting in pipeFittings)
            {
                if (pipeFitting.PredefinedType == IfcPipeFittingTypeEnum.BEND)
                {
                    bends.Add(pipeFitting);
                    continue;
                }
            }

            return bends.ToArray();
        }

        private static IfcCadBendEntity[] CreateBendEntities(IEnumerable<IfcPipeFitting> pipeFittings, IfcAbstractSegmentEntity[] segmentEntities)
        {
            return pipeFittings
                .Select(item => IfcBendEntityExtensions.CreateFromIfc(item, segmentEntities))
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
        
        #endif
    }
}