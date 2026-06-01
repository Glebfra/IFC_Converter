using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Importer.Entities.Topologies
{
    internal sealed class MissingPipeAugmenter : ITopologyAugmenter
    {
        private readonly VectorComparer _comparer;
        private readonly double _defaultDiameter;

        public MissingPipeAugmenter(VectorComparer comparer, double defaultDiameter)
        {
            _comparer = comparer;
            _defaultDiameter = defaultDiameter;
        }

        public IReadOnlyCollection<ITopologyEntity> Augment(IReadOnlyCollection<ITopologyEntity> topologies)
        {
            List<ITopologyEntity> result = topologies.ToList();
            
            foreach (ITopologyEntity topology in topologies)
            {
                IEntityProxy proxy = topology.Proxy;
                if (proxy is not IFittingProxy fitting)
                    continue;
                
                ProxyEntityAttribute attribute = proxy.GetType().GetCustomAttribute<ProxyEntityAttribute>();
                if (topology.ConnectedProxies.Count == attribute.ConnectionsCount)
                    continue;

                double? diameter = null;
                IReadOnlyCollection<IEntityProxy> connectedProxies = topology.ConnectedProxies;
                IEnumerable<ISegmentProxy> connectedSegments = connectedProxies.OfType<ISegmentProxy>();
                Vector<double>[] boundary = fitting.Boundary.ToArray();
                Vector<double>[] unresolvedBoundary = boundary
                    .Where(bound => !connectedSegments.Any(segment => _comparer.Equals(bound, segment.Position) ||
                                                                      _comparer.Equals(bound, segment.EndPosition)))
                    .ToArray();
                
                foreach (Vector<double> unresolvedBound in unresolvedBoundary)
                {
                    Vector<double> projection = unresolvedBound - fitting.Position;
                    double length = projection.L2Norm();
                    Vector<double> direction = projection / length;

                    VirtualPipeSegmentProxy virtualSegment = new VirtualPipeSegmentProxy(
                        diameter ?? _defaultDiameter,
                        length,
                        fitting.Position,
                        direction
                    );

                    ITopologyNodeEntity[] virtualSegmentTopologyNodes = new ITopologyNodeEntity[]
                    {
                        new TopologyNode(fitting.Position), new TopologyNode(unresolvedBound)
                    };
                    IEntityProxy[] virtualSegmentConnectedProxies = new IEntityProxy[]
                    {
                        fitting
                    };
                    TopologyEntity virtualSegmentTopology = new TopologyEntity(virtualSegment, virtualSegmentTopologyNodes, virtualSegmentConnectedProxies);
                    result.Add(virtualSegmentTopology);
                }
            }
            
            return result;
        }
    }
}