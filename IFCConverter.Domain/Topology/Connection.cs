using IFCConverter.Domain.Identity;

namespace IFCConverter.Domain.Topology
{
    public sealed class Connection
    {
        public ConnectionId Id { get; }
        public ConnectionType Type { get; }
        
        public Port PortA { get; }
        public Port PortB { get; }
        
        internal Connection(ConnectionId id, Port portA, Port portB, ConnectionType type)
        {
            Id = id;
            PortA = portA;
            PortB = portB;
            Type = type;
        }
    }
}