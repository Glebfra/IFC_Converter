using System;

namespace IFCConverter.Domain.Identity
{
    public readonly struct PortId : IEquatable<PortId>
    {
        public Guid Value { get; }

        public PortId(Guid value)
        {
            Value = value;
        }

        public static PortId New()
        {
            return new PortId(Guid.NewGuid());
        }

        public bool Equals(PortId other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is PortId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(PortId left, PortId right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PortId left, PortId right)
        {
            return !left.Equals(right);
        }
    }
}