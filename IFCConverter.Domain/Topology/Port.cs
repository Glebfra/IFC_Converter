using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Domain.Topology
{
    public sealed class Port
    {
        public PortId Id { get; }
        public EntityId Owner { get; }
        public Vector<double> Position { get; internal set; }
        public Vector<double> Direction { get; internal set; }
        public PortRole Role { get; internal set; }

        public PortMetadata Metadata { get; } = new PortMetadata();
        
        internal Port(PortId id, EntityId owner)
        {
            Id = id;
            Owner = owner;
        }

        public void SetGeometry(Vector<double> position, Vector<double> direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}