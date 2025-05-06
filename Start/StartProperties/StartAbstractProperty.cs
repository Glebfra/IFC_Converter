using System;
using System.Diagnostics;

namespace Start.StartProperties
{
    [DebuggerDisplay("Start: {StartProperty} ({StartUnit}), SI: {SIProperty} ({SIUnit})")]
    public class StartAbstractProperty<T> : IStartProperty<T>
    {
        public T StartProperty { get; }
        public T SIProperty { get; }

        public virtual string StartUnit { get; } = string.Empty;
        public virtual string SIUnit { get; } = string.Empty;

        public StartAbstractProperty(T startProperty)
        {
            StartProperty = startProperty;
            SIProperty = ConvertFromStart(startProperty);
        }
        
        protected virtual T ConvertFromStart(T startProperty)
        {
            return startProperty;
        }

        protected virtual T ConvertFromSI(T siProperty)
        {
            return siProperty;
        }

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