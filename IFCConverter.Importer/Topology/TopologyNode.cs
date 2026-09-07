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

        public bool Equals(ITopologyNodeEntity other)
        {
            return Position.Equals(other.Position);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != GetType())
                return false;
            return Equals((ITopologyNodeEntity)obj);
        }

        public override int GetHashCode()
        {
            return Position.GetHashCode();
        }
    }
}