using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Domain
{
    public class EngineeringModel
    {
        private readonly Dictionary<Id<ConnectionId>, Connection> _connections = new Dictionary<Id<ConnectionId>, Connection>();
        private readonly Dictionary<Id<EntityId>, Entity> _entities = new Dictionary<Id<EntityId>, Entity>();
        private readonly Dictionary<Id<PortId>, Port> _ports = new Dictionary<Id<PortId>, Port>();

        public IReadOnlyCollection<Entity> Entities => _entities.Values;
        public IReadOnlyCollection<Port> Ports => _ports.Values;
        public IReadOnlyCollection<Connection> Connections => _connections.Values;

        public void Add(Entity entity)
        {
            if (_entities.ContainsKey(entity.Id))
                throw new InvalidOperationException($"Entity {entity.Id} is already exists");
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

            Connection connection = new Connection(Id<ConnectionId>.New(), a, b, type);
            _connections.Add(connection.Id, connection);

            return connection;
        }

        [Pure]
        public Entity GetEntity(Id<EntityId> id)
        {
            return _entities[id];
        }

        [Pure]
        public Port GetPort(Id<PortId> id)
        {
            return _ports[id];
        }

        [Pure]
        public Connection GetConnection(Id<ConnectionId> id)
        {
            return _connections[id];
        }

        [Pure]
        public IEnumerable<Connection> GetConnections(Id<PortId> id)
        {
            return _connections
                .Where(connection => connection.Value.PortA.Id == id || connection.Value.PortB.Id == id)
                .Select(connection => connection.Value);
        }

        [Pure]
        public IEnumerable<Connection> GetConnections(Id<EntityId> id)
        {
            return GetEntity(id).Ports.SelectMany(port => GetConnections(port.Id));
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