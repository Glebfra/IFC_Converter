using System;
using System.Diagnostics;

namespace Entities.Entities.Properties
{
    [DebuggerDisplay("Start: {StartProperty} ({StartUnit}), SI: {SIProperty} ({SIUnit})")]
    public abstract class AbstractProperty<T> : IProperty<T>
    {
        public T StartProperty { get; protected set; }
        public T SIProperty { get; protected set; }

        public virtual string StartUnit { get; } = string.Empty;
        public virtual string SIUnit { get; } = string.Empty;

        protected AbstractProperty() {}

        protected AbstractProperty(T startProperty)
        {
            StartProperty = startProperty;
            SIProperty = ConvertFromStart(startProperty);
        }
        
        public AbstractProperty(T startProperty, T siProperty)
        {
            StartProperty = startProperty;
            SIProperty = siProperty;
        }

        protected abstract T ConvertFromStart(T startProperty);

        protected abstract T ConvertFromSI(T siProperty);

        public Type GetPropertyType()
        {
            return typeof(T);
        }

        public override string ToString()
        {
            return $"{SIProperty.ToString()} {SIUnit}";
        }
    }
}