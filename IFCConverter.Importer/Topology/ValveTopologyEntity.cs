using System.Collections.Generic;
using System.Linq;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Importer.Topology
{
    internal sealed class ValveTopologyEntity : TopologyEntity, ISegmentAugmentableTopologyEntity, IFittingTopologyEntity
    {
        private const double DoubleTolerance = 1e-3;
        private static readonly VectorComparer Comparer = new(DoubleTolerance);

        private readonly double _length;

        public ValveTopologyEntity(IBoundaryProxy proxy, IReadOnlyCollection<ITopologyNodeEntity> nodes)
            : base(proxy, nodes)
        {
            _length = (Proxy.Boundary.ElementAt(1) - Proxy.Boundary.ElementAt(0)).L2Norm();
        }

        public ITopologyNodeEntity Node => Nodes.ElementAt(0);

        public override IStartEntity ToStartEntity()
        {
            StartValveEntity valveEntity = (StartValveEntity)base.ToStartEntity();
            valveEntity.Length.CreateFromSI(_length);
            return valveEntity;
        }

        public void Augment()
        {
            ITopologyNodeEntity valveNode = Nodes.ElementAt(0);

            IEnumerable<SegmentTopologyEntity> segmentTopologyEntities = Connected.OfType<SegmentTopologyEntity>();
            foreach (SegmentTopologyEntity segmentTopologyEntity in segmentTopologyEntities)
            {
                ITopologyNodeEntity segmentStartNode = segmentTopologyEntity.Nodes.ElementAt(0);
                ITopologyNodeEntity segmentEndNode = segmentTopologyEntity.Nodes.ElementAt(1);

                if (Comparer.NearerThan(segmentStartNode.Position, segmentEndNode.Position, valveNode.Position))
                {
                    segmentStartNode = valveNode;
                }
                else
                {
                    segmentEndNode = valveNode;
                }

                Vector<double> projection = segmentEndNode.Position - segmentStartNode.Position;

                segmentTopologyEntity.Augment(segmentStartNode, segmentEndNode, projection);
            }
        }
    }
}