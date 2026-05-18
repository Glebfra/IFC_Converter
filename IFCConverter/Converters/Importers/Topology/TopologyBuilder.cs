using System.Collections.Generic;
using System.Linq;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Converters.Importers.Topology
{
    internal sealed class TopologyBuilder
    {
        private readonly Dictionary<Vector<double>, TopologyNode> _nodes;
        private readonly Dictionary<TopologyNode, TopologyNode> _nodesMap =
            new Dictionary<TopologyNode, TopologyNode>();

        public List<ElementTopologyInfo> Elements { get; } = new List<ElementTopologyInfo>();
        public IReadOnlyCollection<TopologyNode> Nodes => _nodesMap.Values.Distinct().ToArray();

        public TopologyBuilder(IEnumerable<IEntityProxy> proxies, double tolerance)
        {
            _nodes = new Dictionary<Vector<double>, TopologyNode>(new VectorComparer(tolerance));
            AddElements(proxies, tolerance);
        }

        private void AddElements(IEnumerable<IEntityProxy> proxies, double tolerance)
        {
            IEnumerable<IFittingProxy> fittingProxies = proxies.OfType<IFittingProxy>();
            foreach (IFittingProxy fittingProxy in fittingProxies)
            {
                Vector<double> fittingPosition = fittingProxy.Position;
                
                TopologyNode fittingNode = new TopologyNode(fittingPosition);
                fittingNode.ConnectedElements.Add(fittingProxy);

                IEnumerable<IEntityProxy> connectedEntities = GetConnectedEntities(fittingProxy, proxies, tolerance);
                fittingNode.ConnectedElements.AddRange(connectedEntities);
                
                _nodes.Add(fittingPosition, fittingNode);
                _nodesMap.Add(fittingNode, fittingNode);
                
                Elements.Add(new ElementTopologyInfo(fittingProxy, new []{ fittingNode }));
                
                foreach (IEntityProxy connectedEntity in connectedEntities)
                {
                    if (connectedEntity is not ITopologyProxy topologyProxy)
                        continue;

                    IEnumerable<Vector<double>> boundaryPoints = topologyProxy.GetBoundaryPoints();
                }
            }

            foreach (IEntityProxy proxy in proxies)
            {
                if (proxy is not ITopologyProxy topologyProxy)
                    return;

                IEnumerable<IEntityProxy> connectedProxies = GetConnectedEntities(topologyProxy, proxies, tolerance);

                List<TopologyNode> nodes = new List<TopologyNode>();
                if (proxy is IFittingProxy fittingProxy)
                {
                    TopologyNode fittingNode = new TopologyNode(fittingProxy.Position);
                    nodes.Add(fittingNode);
                }
                
                IEnumerable<Vector<double>> boundary = topologyProxy.GetBoundaryPoints();
            }
        }

        private static IEnumerable<IEntityProxy> GetConnectedEntities(
            ITopologyProxy topologyProxy, 
            IEnumerable<IEntityProxy> proxies,
            double tolerance)
        {
            VectorComparer comparer = new VectorComparer(tolerance);
            List<IEntityProxy> result = new List<IEntityProxy>();

            IEnumerable<ITopologyProxy> topologyProxies = proxies.OfType<ITopologyProxy>();
            IEnumerable<Vector<double>> firstBoundaryPoints = topologyProxy.GetBoundaryPoints();
            
            foreach (ITopologyProxy otherTopologyProxy in topologyProxies)
            {
                IEnumerable<Vector<double>> secondBoundaryPoints = otherTopologyProxy.GetBoundaryPoints();
                
                bool isConnected = firstBoundaryPoints.Any(p1 =>
                    secondBoundaryPoints.Any(p2 =>
                        comparer.Equals(p1, p2)
                    )
                );
                
                if (isConnected)
                    result.Add((IEntityProxy)otherTopologyProxy);
            }

            return result;
        }

        private void AddElement(IEntityProxy proxy)
        {
            if (proxy is not ITopologyProxy topologyProxy)
                return;

            List<TopologyNode> nodes = new List<TopologyNode>();
            foreach (Vector<double> point in topologyProxy.GetBoundaryPoints())
            {
                TopologyNode node = GetOrCreateNode(point);
                node.ConnectedElements.Add(proxy);
                nodes.Add(node);
            }
            Elements.Add(new ElementTopologyInfo(proxy, nodes));
        }

        private TopologyNode GetOrCreateNode(Vector<double> point)
        {
            if (_nodes.TryGetValue(point, out TopologyNode? node))
                return node;

            node = new TopologyNode(point);
            _nodes.Add(point, node);
            return node;
        }
    }
}