using System.Linq;
using IFC;
using IFC.Entities.Segments;
using Newtonsoft.Json;
using Start;
using Start.API;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;

namespace IfcConverter
{
    public class IfcConverter
    {
        public void Import(string filePath, string ctpFilePath)
        {
            using (IFCProject ifcProject = IFCProject.OpenProject(filePath))
            {
                IModel model = ifcProject.GetModel();
                
                IfcPipeSegment[] pipeSegments = model.Instances
                    .OfType<IfcPipeSegment>()
                    .Where(item => 
                        item.Tag != StartElementType.RIGID_ELEMENT.ToString() && 
                        item.Tag != StartElementType.FLEXIBLE_ELEMENT.ToString())
                    .ToArray();
                IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipeSegments.Length];
                for (int i = 0; i < pipeSegments.Length; i++)
                {
                    pipeSegmentEntities[i] = IfcPipeSegmentEntity.CreateFromIfc(pipeSegments[i]);
                }

                using (StartProject startProject = StartProject.OpenProject(ctpFilePath))
                {
                    foreach (IfcPipeSegmentEntity ifcPipeSegmentEntity in pipeSegmentEntities)
                    {
                        string startPipeJson = JsonConvert.SerializeObject(ifcPipeSegmentEntity.StartAbstractEntity);
                        using (StartBaseRoot pipeElement = startProject.AddElement(StartElementType.PIPE_ELEMENT, out int pipeIndex))
                        {
                            pipeElement.SetDataJson(0, startPipeJson);
                            
                            StartBaseRoot[] nodeEntities = new StartBaseRoot[2];
                            int[] nodeIndexes = new int[2];
                            for (int i = 0; i < nodeEntities.Length; i++)
                            {
                                string startNodeJson = JsonConvert.SerializeObject(ifcPipeSegmentEntity.NodeEntities[i].NodeEntity);
                                nodeEntities[i] = startProject.AddElement(StartElementType.NODE, out int nodeIndex);
                                nodeIndexes[i] = nodeIndex;
                                nodeEntities[i].SetDataJson(0, startNodeJson);
                            }
                            pipeElement.SetSNode(nodeIndexes[0]);
                            pipeElement.SetENode(nodeIndexes[1]);
                            
                            foreach (StartBaseRoot nodeEntity in nodeEntities)
                            {
                                nodeEntity.Dispose();
                            }
                        }
                    }

                    string json = startProject.GetDataJson();
                }
            }
        }
    }
}