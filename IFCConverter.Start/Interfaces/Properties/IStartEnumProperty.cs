using System;

namespace IFCConverter.Start.Interfaces
{
    public interface IStartEnumProperty : IStartProperty
    {
        object GetEnumValue();
    }

    public interface IStartEnumProperty<T> : IStartEnumProperty
        where T : Enum
    {
        T EnumValue { get; set; }
    }
}