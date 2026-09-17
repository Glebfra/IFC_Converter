using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Domain.Entities
{
    public abstract class Entity
    {
        private readonly List<Port> _ports = new List<Port>();

        protected Entity(EntityId id)
        {
            Id = id;
        }

        public EntityId Id { get; }
        public EntityMetadata Metadata { get; } = new EntityMetadata();
        public IReadOnlyCollection<Port> Ports => _ports;
        public abstract IReadOnlyCollection<FixedVector<Dim3>> Positions { get; }

        protected Port CreatePort(FixedVector<Dim3> position = null, FixedVector<Dim3> direction = null, PortRole role = PortRole.Connection)
        {
            Port port = new Port(PortId.New(), Id);
            port.Position = position ?? FixedVector<Dim3>.Zeros();
            port.Direction = direction ?? FixedVector<Dim3>.Zeros();
            port.Role = role;
            _ports.Add(port);

            return port;
        }
    }
}