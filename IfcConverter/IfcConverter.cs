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
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IfcConverter
{
    internal class IfcConverter
    {
        private int _nodeIndex = 1;
        private Dictionary<IfcNodeEntity, StartObject> _nodeEntities = new Dictionary<IfcNodeEntity, StartObject>();

        public void Import(string filePath, string ctpFilePath)
        {
            using (IFCProject ifcProject = IFCProject.OpenProject(filePath))
            {
                IModel model = ifcProject.GetModel();

                IfcPipeSegment[] pipeSegments = GetPipeSegments(model);
                IfcPipeSegmentEntity[] pipeSegmentEntities = CreatePipeSegmentEntities(pipeSegments);

                IfcPipeFitting[] pipeFittings = GetPipeFittings(model);
                IfcPipeFitting[] junctions = FilterJunctions(pipeFittings);
                IfcWeldedTeeEntity[] weldedTeeEntities = CreateWeldedTeeEntities(junctions, pipeSegmentEntities);
                
                using (StartProject startProject = StartProject.OpenProject(ctpFilePath))
                {
                    GenerateWeldedTees(startProject, weldedTeeEntities);
                    GeneratePipeSegments(startProject, pipeSegmentEntities);
                }
            }

            foreach (KeyValuePair<IfcNodeEntity, StartObject> kvp in _nodeEntities)
            {
                kvp.Value.Object.Dispose();
            }
            _nodeEntities = null;
        }

        private void GenerateWeldedTees(StartProject startProject, IfcWeldedTeeEntity[] weldedTeeEntities)
        {
            foreach (IfcWeldedTeeEntity ifcWeldedTeeEntity in weldedTeeEntities)
            {
                StartTeeEntity startTeeEntity = (StartTeeEntity)ifcWeldedTeeEntity.StartAbstractEntity;
                string startTeeJson = JsonConvert.SerializeObject(startTeeEntity);

                using (StartBaseRoot startTeeObject = startProject.AddElement(StartElementType.WELDED_TEE, out int teeIndex))
                {
                    startTeeObject.SetDataJson(0, startTeeJson);

                    IfcNodeEntity ifcNodeEntity = ifcWeldedTeeEntity.NodeEntity;
                    if (!_nodeEntities.ContainsKey(ifcNodeEntity))
                    {
                        string startNodeJson = JsonConvert.SerializeObject(ifcNodeEntity.NodeEntity);
                        StartBaseRoot newNodeEntityObject = startProject.AddElement(StartElementType.NODE, out int nodeIndex);
                        newNodeEntityObject.SetDataJson(0, startNodeJson);
                        newNodeEntityObject.SetName((_nodeIndex++).ToString());
                        _nodeEntities.Add(ifcNodeEntity, new StartObject() {Object = newNodeEntityObject, Index = nodeIndex});
                    }
                    
                    startTeeObject.SetSNode(_nodeEntities[ifcNodeEntity].Index);
                    
                    foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in ifcWeldedTeeEntity.AbstractSegmentEntities)
                    {
                        XbimVector3D displacement = ifcAbstractSegmentEntity.ReplaceNearestNode(ifcNodeEntity);
                        // StartPipeEntity pipeEntity = (StartPipeEntity)ifcAbstractSegmentEntity.StartAbstractEntity;
                        // pipeEntity.ProjectionAlongOXAxis = new LengthProperty(pipeEntity.ProjectionAlongOXAxis.SIProperty + displacement.X);
                        // pipeEntity.ProjectionAlongOYAxis = new LengthProperty(pipeEntity.ProjectionAlongOYAxis.SIProperty + displacement.Y);
                        // pipeEntity.ProjectionAlongOZAxis = new LengthProperty(pipeEntity.ProjectionAlongOZAxis.SIProperty + displacement.Z);
                    }
                }
            }
        }

        private void GeneratePipeSegments(StartProject startProject, IfcPipeSegmentEntity[] pipeSegmentEntities)
        {
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in pipeSegmentEntities)
            {
                StartPipeEntity startPipeEntity = (StartPipeEntity)ifcPipeSegmentEntity.StartAbstractEntity;
                string startPipeJson = JsonConvert.SerializeObject(startPipeEntity);

                using (StartBaseRoot startPipeObject = startProject.AddElement(StartElementType.PIPE_ELEMENT, out int pipeIndex))
                {
                    startPipeObject.SetDataJson(0, startPipeJson);

                    StartObject[] nodeEntityObjects = new StartObject[2];
                    for (int i = 0; i < 2; i++)
                    {
                        IfcNodeEntity ifcNodeEntity = ifcPipeSegmentEntity.NodeEntities[i];
                        if (!_nodeEntities.ContainsKey(ifcNodeEntity))
                        {
                            string startNodeJson = JsonConvert.SerializeObject(ifcNodeEntity.NodeEntity);
                            StartBaseRoot newNodeEntityObject = startProject.AddElement(StartElementType.NODE, out int nodeIndex);
                            newNodeEntityObject.SetDataJson(0, startNodeJson);
                            newNodeEntityObject.SetName((_nodeIndex++).ToString());
                            _nodeEntities.Add(ifcNodeEntity, new StartObject {Object = newNodeEntityObject, Index = nodeIndex});
                        }

                        nodeEntityObjects[i] = _nodeEntities[ifcNodeEntity];
                    }
                    startPipeObject.SetSNode(nodeEntityObjects[0].Index);
                    startPipeObject.SetENode(nodeEntityObjects[1].Index);
                }
            }
        }

        private static IfcPipeSegment[] GetPipeSegments(IModel model)
        {
            return model.Instances
                .OfType<IfcPipeSegment>()
                .ToArray();
        }

        private static IfcPipeFitting[] GetPipeFittings(IModel model)
        {
            return model.Instances
                .OfType<IfcPipeFitting>()
                .ToArray();
        }

        private static IfcPipeFitting[] FilterJunctions(IfcPipeFitting[] pipeFittings)
        {
            return pipeFittings
                .Where(item => item.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                .ToArray();
        }

        private static IfcWeldedTeeEntity[] CreateWeldedTeeEntities(IEnumerable<IfcPipeFitting> pipeFittings, IfcAbstractSegmentEntity[] segmentEntities)
        {
            return pipeFittings
                .Select(item => IfcWeldedTeeEntity.CreateFromIfc(item, segmentEntities))
                .ToArray();
        }

        private static IfcPipeSegmentEntity[] CreatePipeSegmentEntities(IEnumerable<IfcPipeSegment> pipeSegments)
        {
            return pipeSegments
                .Select(IfcPipeSegmentEntity.CreateFromIfc)
                .ToArray();
        }
    }
}