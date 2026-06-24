using System.Diagnostics;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.Topology
{
    [DebuggerDisplay("Node: ({Position[0]}, {Position[1]}, {Position[2]})")]
    internal class TopologyNode : ITopologyNodeEntity
    {

        public TopologyNode(Vector<double> position)
        {
            Position = position;
        }

        public Vector<double> Position { get; }
    }
}