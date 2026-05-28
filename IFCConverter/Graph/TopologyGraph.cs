using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;
using Utils;

namespace IFCConverter.Graph
{
    internal sealed class TopologyGraph
    {
        public IReadOnlyDictionary<Vector<double>, ConnectionNode> Nodes => _nodes;
        
        private readonly Dictionary<Vector<double>, ConnectionNode> _nodes;

        public TopologyGraph(VectorComparer comparer)
        {
            _nodes = new Dictionary<Vector<double>, ConnectionNode>(comparer);
        }

        public ConnectionNode GetOrCreateNode(Vector<double> position)
        {
            return _nodes.GetOrAdd(position, vector => new ConnectionNode(position));
        }
    }
}