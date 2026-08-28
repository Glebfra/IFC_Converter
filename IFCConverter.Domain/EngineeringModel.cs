using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain
{
    public class EngineeringModel
    {
        private readonly Dictionary<ConnectionId, Connection> _connections = new Dictionary<ConnectionId, Connection>();
        private readonly Dictionary<EntityId, Entity> _entities = new Dictionary<EntityId, Entity>();
        private readonly Dictionary<PortId, Port> _ports = new Dictionary<PortId, Port>();

        public IReadOnlyCollection<Entity> Entities => _entities.Values;
        public IReadOnlyCollection<Port> Ports => _ports.Values;
        public IReadOnlyCollection<Connection> Connections => _connections.Values;

        public void Add(Entity entity)
        {
            if (_entities.ContainsKey(entity.Id))
                throw new InvalidOperationException($"Entity {entity.Id}  already exists");
            _entities.Add(entity.Id, entity);

            foreach (Port port in entity.Ports)
            {
                if (_ports.ContainsKey(port.Id))
                    throw new InvalidOperationException($"Port {port.Id} is already added");
                _ports.Add(port.Id, port);
            }
        }

        public Connection Connect(Port a, Port b, ConnectionType type)
        {
            ValidatePort(a);
            ValidatePort(b);

            if (a.Owner == b.Owner)
                throw new InvalidOperationException("An entity cannot be connected to itself");

            Connection connection = new Connection(ConnectionId.New(), a, b, type);
            _connections.Add(connection.Id, connection);

            return connection;
        }

        public Entity GetEntity(EntityId id)
        {
            return _entities[id];
        }

        public Port GetPort(PortId id)
        {
            return _ports[id];
        }

        public Connection GetConnection(ConnectionId id)
        {
            return _connections[id];
        }

        private void ValidatePort(Port port)
        {
            if (!_ports.TryGetValue(port.Id, out Port existing))
                throw new InvalidOperationException($"Port {port.Id} does not belong to this model");

            if (!ReferenceEquals(existing, port))
                throw new InvalidOperationException($"Port {port.Id} belongs to another model");
        }
    }
}