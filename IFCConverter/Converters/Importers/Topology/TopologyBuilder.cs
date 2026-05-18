using System.Collections.Generic;
using System.Linq;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Utils;

namespace IFCConverter.Converters.Importers.Topology
{
    internal sealed class TopologyBuilder
    {
        private readonly double _tolerance;
        private readonly VectorComparer _comparer;
        
        private readonly Dictionary<Vector<double>, TopologyNode> _nodes;
        private readonly Dictionary<Vector<double>, TopologyNode> _fittingNodes;

        public List<ElementTopologyInfo> Elements { get; } = new List<ElementTopologyInfo>();
        public IReadOnlyCollection<TopologyNode> Nodes => _nodes.Values.Distinct().ToArray();

        public TopologyBuilder(IEnumerable<IEntityProxy> proxies, double tolerance)
        {
            _tolerance = tolerance;
            _comparer = new VectorComparer(tolerance);
            
            _nodes = new Dictionary<Vector<double>, TopologyNode>(new VectorComparer(tolerance));
            _fittingNodes = new Dictionary<Vector<double>, TopologyNode>(new VectorComparer(tolerance));
            
            AddElements(proxies);
        }

        #if true
        private void AddElements(IEnumerable<IEntityProxy> proxies)
        {
            foreach (IEntityProxy proxy in proxies)
            {
                IEnumerable<Vector<double>> boundary = proxy.Boundary;
            }
        }
        #else
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
        #endif

        private static IEnumerable<IEntityProxy> GetConnectedEntities(
            IEntityProxy entityProxy, 
            IEnumerable<IEntityProxy> proxies,
            double tolerance)
        {
            VectorComparer comparer = new VectorComparer(tolerance);
            List<IEntityProxy> result = new List<IEntityProxy>();
            
            IEnumerable<Vector<double>> firstBoundaryPoints = entityProxy.Boundary;
            
            foreach (IEntityProxy proxy in proxies)
            {
                IEnumerable<Vector<double>> secondBoundaryPoints = proxy.Boundary;

                bool isConnected = firstBoundaryPoints.Any(p1 =>
                    secondBoundaryPoints.Any(p2 =>
                        comparer.Equals(p1, p2)
                    )
                );
                
                if (isConnected)
                    result.Add(proxy);
            }

            return result;
        }
    }
}