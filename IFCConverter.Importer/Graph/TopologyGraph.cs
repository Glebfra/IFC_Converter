using System.Collections.Generic;
using IFCConverter.Utils.Collections;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Graph
{
    internal sealed class TopologyGraph
    {
        private readonly Dictionary<Vector<double>, ConnectionNode> _nodes;

        public TopologyGraph(VectorComparer comparer)
        {
            _nodes = new Dictionary<Vector<double>, ConnectionNode>(comparer);
        }

        public IReadOnlyDictionary<Vector<double>, ConnectionNode> Nodes => _nodes;

        public ConnectionNode GetOrCreateNode(Vector<double> position)
        {
            return _nodes.GetOrAdd(position, vector => new ConnectionNode(position));
        }
    }
}