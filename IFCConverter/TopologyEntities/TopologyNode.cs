using System.Diagnostics;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.TopologyEntities
{
    [DebuggerDisplay("Node: ({Position[0]}, {Position[1]}, {Position[2]})")]
    internal readonly struct TopologyNode : ITopologyNodeEntity
    {
        public Vector<double> Position { get; }

        public TopologyNode(Vector<double> position)
        {
            Position = position;
        }
    }
}