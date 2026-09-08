using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    [TopologyEntity]
    internal class SegmentTopologyEntity : TopologyEntity, ISegmentTopologyEntity
    {

        private Vector<double> _resolvedStartPosition;

        public SegmentTopologyEntity(
            IBoundaryProxy proxy,
            IReadOnlyCollection<ITopologyNodeEntity> nodes)
            : base(proxy, nodes)
        {
            Vector<double> resolvedStartPosition = nodes.ElementAt(0).Position;
            Vector<double> resolvedEndPosition = nodes.ElementAt(1).Position;

            ISegmentProxy segmentProxy = (ISegmentProxy)proxy.Proxy;
            Vector<double> segmentProjection = segmentProxy.Direction * segmentProxy.Length;
            Vector<double> resolvedProjection = resolvedEndPosition - resolvedStartPosition;

            _resolvedStartPosition = resolvedStartPosition;
            Projection = segmentProjection.Normalize(2) * resolvedProjection.L2Norm();
        }

        public Vector<double> Projection { get; private set; }

        public override IStartEntity ToStartEntity()
        {
            StartPipeEntity startSegmentEntity = (StartPipeEntity)base.ToStartEntity();
            startSegmentEntity.StartPosition = _resolvedStartPosition;
            startSegmentEntity.Projection = Projection;

            return startSegmentEntity;
        }

        public void Augment(ITopologyNodeEntity startNode, ITopologyNodeEntity endNode, Vector<double> projection)
        {
            Projection = projection;
            _resolvedStartPosition = startNode.Position;

            Nodes = new[]
            {
                startNode, endNode
            };
        }
    }
}