using System.Collections.Generic;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Converters.Importers.Topology
{
    internal sealed class TopologyNode
    {
        public Vector<double> Position { get; }
        public List<IEntityProxy> ConnectedElements { get; } = new List<IEntityProxy>();
        
        public TopologyNode(Vector<double> position)
        {
            Position = position;
        }
    }
}