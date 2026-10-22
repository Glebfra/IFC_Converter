using System.Collections.Generic;
using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Topology
{
    public sealed class Connection
    {
        internal Connection(Id<ConnectionId> id, Port portA, Port portB, ConnectionType type)
        {
            Id = id;
            PortA = portA;
            PortB = portB;
            Type = type;
        }

        public Id<ConnectionId> Id { get; }
        public ConnectionType Type { get; }

        public Port PortA { get; }
        public Port PortB { get; }

        public IEnumerable<Port> Ports
        {
            get
            {
                yield return PortA;
                yield return PortB;
            }
        }
    }
}