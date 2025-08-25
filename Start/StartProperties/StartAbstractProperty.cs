using System;
using System.Diagnostics;

namespace Start.StartProperties
{
    [DebuggerDisplay("Start: {StartProperty} ({StartUnit}), SI: {SIProperty} ({SIUnit})")]
    public abstract class StartAbstractProperty<T> : IStartProperty<T>
    {
        public T StartProperty { get; protected set; }
        public T SIProperty { get; protected set; }

        public virtual string StartUnit { get; } = string.Empty;
        public virtual string SIUnit { get; } = string.Empty;

        protected StartAbstractProperty() {}

        protected StartAbstractProperty(T startProperty)
        {
            StartProperty = startProperty;
            SIProperty = ConvertFromStart(startProperty);
        }
        
        public StartAbstractProperty(T startProperty, T siProperty)
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