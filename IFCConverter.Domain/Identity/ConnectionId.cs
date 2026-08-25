using System;

namespace IFCConverter.Domain.Identity
{
    public readonly struct ConnectionId : IEquatable<ConnectionId>
    {
        public Guid Value { get; }
        
        public ConnectionId(Guid value)
        {
            Value = value;
        }

        public static ConnectionId New()
        {
            return new ConnectionId(Guid.NewGuid());
        }
        
        public bool Equals(ConnectionId other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is ConnectionId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(ConnectionId left, ConnectionId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ConnectionId left, ConnectionId right)
        {
            return !left.Equals(right);
        }
    }
}