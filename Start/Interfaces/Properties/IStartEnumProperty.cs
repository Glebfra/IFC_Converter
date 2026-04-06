using System;

namespace Start.Interfaces
{
    public interface IStartEnumProperty<T> : IStartProperty
        where T : Enum
    {
        T EnumValue { get; set; }
    }
}