using System;

namespace IFCConverter.Domain.Identity
{
    public interface IId
    {
    }

    public readonly struct PortId : IId
    {
    }

    public readonly struct EntityId : IId
    {
    }

    public readonly struct ConnectionId : IId
    {
    }

    public readonly struct Id<TId> : IEquatable<Id<TId>>
        where TId : IId
    {
        public Guid Value { get; }

        public Id(Guid value)
        {
            Value = value;
        }

        public static Id<TId> New()
        {
            return new Id<TId>(Guid.NewGuid());
        }

        public bool Equals(Id<TId> other)
        {
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is Id<TId> other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public static bool operator ==(Id<TId> left, Id<TId> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Id<TId> left, Id<TId> right)
        {
            return !left.Equals(right);
        }
    }
}