using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Importer.Topology
{
    internal class SegmentTopologyEntity : TopologyEntity, ISegmentTopologyEntity
    {
        private Vector<double> _resolvedProjection;

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
            _resolvedProjection = segmentProjection.Normalize(2) * resolvedProjection.L2Norm();
        }

        public void Augment(ITopologyNodeEntity startNode, ITopologyNodeEntity endNode, Vector<double> projection)
        {
            _resolvedProjection = projection;
            _resolvedStartPosition = startNode.Position;

            Nodes = new ITopologyNodeEntity[]
            {
                startNode, endNode
            };
        }

        public override IStartEntity ToStartEntity()
        {
            StartPipeEntity startSegmentEntity = (StartPipeEntity)base.ToStartEntity();
            startSegmentEntity.StartPosition = _resolvedStartPosition;
            startSegmentEntity.Projection = _resolvedProjection;

            return startSegmentEntity;
        }
    }
}