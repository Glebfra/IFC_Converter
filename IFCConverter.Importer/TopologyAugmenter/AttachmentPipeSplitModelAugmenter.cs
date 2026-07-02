using System.Linq;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.TopologyAugmenter
{
    internal sealed class AttachmentPipeSplitModelAugmenter : ITopologyModelAugmenter
    {
        public void Augment(ref ITopologyModel model)
        {
            foreach (AttachmentTopologyEntity attachmentTopologyEntity in model.Entities.OfType<AttachmentTopologyEntity>().ToArray())
            {
                foreach (SegmentTopologyEntity segmentTopologyEntity in attachmentTopologyEntity.Connected.OfType<SegmentTopologyEntity>().ToArray())
                {
                    ISegmentProxy segmentProxy = (ISegmentProxy)segmentTopologyEntity.Proxy.Proxy;
                    double diameter = segmentProxy.Diameter;

                    ITopologyNodeEntity oldStartNode = segmentTopologyEntity.Nodes.ElementAt(0);
                    ITopologyNodeEntity oldEndNode = segmentTopologyEntity.Nodes.ElementAt(1);

                    ITopologyNodeEntity newEndNode = attachmentTopologyEntity.Node;
                    Vector<double> newProjection = newEndNode.Position - oldStartNode.Position;
                    segmentTopologyEntity.Augment(oldStartNode, newEndNode, newProjection);

                    ITopologyNodeEntity newStartNode = attachmentTopologyEntity.Node;
                    Vector<double> newSegmentProjection = oldEndNode.Position - newStartNode.Position;
                    ISegmentProxy newSegmentProxy = new PipeSegmentProxy(
                        diameter,
                        newSegmentProjection.L2Norm(),
                        newStartNode.Position,
                        newSegmentProjection.Normalize(2)
                    )
                    {
                        Name = $"Part of segment {segmentProxy.Name}"
                    };
                    model.AddEntity(newSegmentProxy);
                }
            }
        }
    }
}