using System;
using Start.Interfaces;

namespace Start.StartProperties
{
    public class EnumProperty<T> : IStartEnumProperty<T>
        where T : Enum
    {
        public T EnumValue { get; set; } = default!;

        public object Write()
        {
            return (int)Convert.ChangeType(EnumValue, typeof(int));
        }

        public object Read(object value)
        {
            Type enumType = typeof(T);
            if (!enumType.IsEnum)
                throw new ArgumentException("T must be an enumerated type");
            if (value == null)
                throw new NullReferenceException(nameof(value));
            int intRawValue = Convert.ToInt32(value);
            string rawValue = Convert.ToString(intRawValue);
            EnumValue = (T)Enum.Parse(enumType, rawValue);

            return EnumValue;
        }
    }
}