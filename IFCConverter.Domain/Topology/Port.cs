using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Topology
{
    public sealed class Port
    {
        internal Port(PortId id, EntityId owner)
        {
            Id = id;
            Owner = owner;
        }

        public PortId Id { get; }
        public EntityId Owner { get; }
        public FixedVector<Dim3> Position { get; internal set; }
        public FixedVector<Dim3> Direction { get; internal set; }
        public PortRole Role { get; internal set; } = PortRole.Connection;

        public PortMetadata Metadata { get; } = new PortMetadata();

        public void SetGeometry(FixedVector<Dim3> position, FixedVector<Dim3> direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}