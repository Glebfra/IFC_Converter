using System;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal sealed class EnumPropertyConverter<TEnum> : AbstractPropertyConverter<IfcValue, TEnum>
        where TEnum : struct, Enum
    {
        public override TEnum ReadTyped(IfcValue source)
        {
            if (Enum.TryParse(source.Value?.ToString(), out TEnum @enum))
                return @enum;

            return default;
        }
    }
}