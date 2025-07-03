using System;

namespace Entities.Entities.Properties
{
    public interface IProperty<out T>
    {
        public T StartProperty { get; }
        public T SIProperty { get; }

        public string StartUnit { get; }
        public string SIUnit { get; }

        public Type GetPropertyType();
    }
}