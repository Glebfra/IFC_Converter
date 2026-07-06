using System.Collections.Generic;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal sealed class AvevaBoolPropertyConverter : AbstractPropertyConverter<IfcValue, bool>
    {
        private static readonly Dictionary<string, bool> BoolMap = new();

        public AvevaBoolPropertyConverter()
        {
            BoolMap["f"] = false;
            BoolMap["fals"] = false;
            BoolMap["false"] = false;

            BoolMap["t"] = true;
            BoolMap["tru"] = true;
            BoolMap["true"] = true;
        }

        public override bool ReadTyped(IfcValue source)
        {
            string value = source.Value.ToString().ToLower();
            return BoolMap.TryGetValue(value, out bool outValue) && outValue;
        }
    }
}