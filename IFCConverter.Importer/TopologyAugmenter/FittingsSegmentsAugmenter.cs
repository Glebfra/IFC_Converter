using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.Proxies;
using IFCConverter.Importer.Topology;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.TopologyAugmenter
{
    internal sealed class FittingsSegmentsAugmenter : ITopologyModelAugmenter
    {
        private readonly BoundaryResolver _boundaryResolver = BoundaryResolver.GetInstance();
        private readonly VectorComparer _comparer;

        public FittingsSegmentsAugmenter(VectorComparer comparer)
        {
            _comparer = comparer;
        }

        public void Augment(ref ITopologyModel model)
        {
            foreach (IFittingTopologyEntity fittingTopologyEntity in model.Entities.OfType<IFittingTopologyEntity>().ToArray())
            {
                foreach (IFittingTopologyEntity connectedFitting in fittingTopologyEntity.Connected.OfType<IFittingTopologyEntity>().ToArray())
                {
                    Vector<double> start = fittingTopologyEntity.Node.Position;
                    Vector<double> end = connectedFitting.Node.Position;
                    Vector<double> projection = end - start;
                    double length = projection.L2Norm();
                    Vector<double> direction = projection / length;

                    PipeSegmentProxy segmentProxy = new PipeSegmentProxy(0.1, length, start, direction)
                    {
                        Name = $"Generated segment for {fittingTopologyEntity.Proxy.Proxy.Name} | {connectedFitting.Proxy.Proxy.Name}"
                    };

                    Vector<double>[] boundary = new Vector<double>[]
                    {
                        start, end
                    };
                    IBoundaryProxy boundaryProxy = new BoundaryProxy(segmentProxy, boundary);
                    IReadOnlyCollection<ITopologyNodeEntity> nodes = boundary.Select(bound => new TopologyNode(bound)).ToArray();
                    ISegmentTopologyEntity segmentTopologyEntity = new SegmentTopologyEntity(boundaryProxy, nodes);
                    
                    connectedFitting.Connect(segmentTopologyEntity);
                    fittingTopologyEntity.Connect(segmentTopologyEntity);
                    model.AddEntity(segmentTopologyEntity);
                }
            }
        }
    }
}