using System.Collections.Generic;
using System.Linq;
using IFC;
using IFC.Entities;
using IFC.Entities.Segments;
using Newtonsoft.Json;
using Start;
using Start.API;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;

namespace IfcConverter
{
    internal struct StartObject
    {
        public StartBaseRoot Object;
        public int Index;
    }
    
    public class IfcConverter
    {
        private Dictionary<IfcNodeEntity, StartObject> _nodeEntities = new Dictionary<IfcNodeEntity, StartObject>();

        public void Import(string filePath, string ctpFilePath)
        {
            using IFCProject ifcProject = IFCProject.OpenProject(filePath);
            using StartProject startProject = StartProject.OpenProject(ctpFilePath);
            
            IModel model = ifcProject.GetModel();

            IfcPipeSegment[] pipeSegments = GetPipeSegments(model);
            IfcPipeSegmentEntity[] pipeSegmentEntities = CreatePipeSegmentEntities(pipeSegments);
            
            foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in pipeSegmentEntities)
            {
                StartPipeEntity startPipeEntity = (StartPipeEntity)ifcPipeSegmentEntity.StartAbstractEntity;
                string startPipeJson = JsonConvert.SerializeObject(startPipeEntity);

                using (StartBaseRoot startPipeObject = startProject.AddElement(StartElementType.PIPE_ELEMENT, out int pipeIndex))
                {
                    startPipeObject.SetDataJson(0, startPipeJson);

                    StartObject[] nodeEntityObjects = new StartObject[2];
                    int[] nodeIndexes = new int[2];
                    for (int i = 0; i < 2; i++)
                    {
                        IfcNodeEntity ifcNodeEntity = ifcPipeSegmentEntity.NodeEntities[i];
                        if (!_nodeEntities.ContainsKey(ifcNodeEntity))
                        {
                            string startNodeJson = JsonConvert.SerializeObject(ifcNodeEntity.NodeEntity);
                            StartBaseRoot newNodeEntityObject = startProject.AddElement(StartElementType.NODE, out int nodeIndex);
                            newNodeEntityObject.SetDataJson(0, startNodeJson);
                            nodeIndexes[i] = nodeIndex;
                            _nodeEntities.Add(ifcNodeEntity, new StartObject {Object = newNodeEntityObject, Index = nodeIndex});
                        }

                        nodeEntityObjects[i] = _nodeEntities[ifcNodeEntity];
                    }
                    startPipeObject.SetSNode(nodeEntityObjects[0].Index);
                    startPipeObject.SetENode(nodeEntityObjects[1].Index);
                }
            }
            
            foreach (KeyValuePair<IfcNodeEntity, StartObject> kvp in _nodeEntities)
            {
                kvp.Value.Object.Dispose();
            }
            _nodeEntities = null;
        }

        private static IfcPipeSegmentEntity[] CreatePipeSegmentEntities(IEnumerable<IfcPipeSegment> pipeSegments)
        {
            return pipeSegments
                .Select(IfcPipeSegmentEntity.CreateFromIfc)
                .ToArray();
        }

        private static IfcPipeSegment[] GetPipeSegments(IModel model)
        {
            return model.Instances
                .OfType<IfcPipeSegment>()
                .Where(item => item.Tag == StartElementType.PIPE_ELEMENT.ToString())
                .ToArray();
        }
    }
}