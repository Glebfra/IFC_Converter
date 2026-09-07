using System.Collections.Generic;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Metadata;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

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
        public abstract IReadOnlyCollection<Vector<double>> Positions { get; }

        protected Port CreatePort(Vector<double> position = null, Vector<double> direction = null, PortRole role = PortRole.Connection)
        {
            Port port = new Port(PortId.New(), Id);
            port.Position = position ?? VectorExtensions.Zero;
            port.Direction = direction ?? VectorExtensions.Zero;
            port.Role = role;
            _ports.Add(port);

            return port;
        }
    }
}