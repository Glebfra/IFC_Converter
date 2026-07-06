using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Start.Interfaces;

namespace Start.StartProperties
{
    [DebuggerDisplay("StartProperty: {StartProperty} {StartUnit}, SIProperty: {SIProperty} {SIUnit}")]
    public abstract class StartValueAbstractProperty<T> :
        IStartValueProperty<T>, IComparable<StartValueAbstractProperty<T>>
        where T : struct, IComparable<T>
    {
        public int CompareTo(StartValueAbstractProperty<T>? other)
        {
            if (other == null)
                return 1;

            return SIProperty.CompareTo(other.SIProperty);
        }

        public T StartProperty { get; private set; }
        public T SIProperty { get; private set; }

        public abstract string StartUnit { get; }
        public abstract string SIUnit { get; }

        public abstract double StartToSIFactor { get; }

        public object GetStartProperty()
        {
            return StartProperty;
        }

        public object GetSIProperty()
        {
            return SIProperty;
        }

        public string GetStartUnit()
        {
            return StartUnit;
        }

        public string GetSIUnit()
        {
            return SIUnit;
        }

        public IStartValueProperty<T> CreateFromStart(T startProperty)
        {
            dynamic property = startProperty;
            StartProperty = startProperty;
            SIProperty = Convert.ChangeType(property * StartToSIFactor, typeof(T));
            return this;
        }

        public IStartValueProperty<T> CreateFromSI(T siProperty)
        {
            dynamic property = siProperty;
            SIProperty = siProperty;
            StartProperty = Convert.ChangeType(property / StartToSIFactor, typeof(T));
            return this;
        }

        [Pure]
        public Type GetGenericType()
        {
            return typeof(T);
        }

        public object Write()
        {
            return StartProperty!;
        }

        [Pure]
        public object Read(object value)
        {
            if (value == null)
                throw new NullReferenceException(nameof(value));
            CreateFromStart((T)Convert.ChangeType(value, typeof(T)));
            return this;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as StartValueAbstractProperty<T>);
        }

        public override string ToString()
        {
            return $"{SIProperty.ToString()} {SIUnit}";
        }

        public static bool operator >(StartValueAbstractProperty<T> left, StartValueAbstractProperty<T> right)
        {
            return left.CompareTo(right) > 0;
        }

        public static bool operator <(StartValueAbstractProperty<T> left, StartValueAbstractProperty<T> right)
        {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >=(StartValueAbstractProperty<T> left, StartValueAbstractProperty<T> right)
        {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <=(StartValueAbstractProperty<T> left, StartValueAbstractProperty<T> right)
        {
            return left.CompareTo(right) <= 0;
        }
    }
}