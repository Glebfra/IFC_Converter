using System;

namespace Start.Interfaces
{
    public interface IStartEnumProperty : IStartProperty
    {
        public object GetEnumValue();
    }

    public interface IStartEnumProperty<T> : IStartEnumProperty
        where T : Enum
    {
        T EnumValue { get; set; }
    }
}