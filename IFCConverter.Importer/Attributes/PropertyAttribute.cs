using System;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    internal class PropertyAttribute : AbstractPropertyItemAttribute
    {
        public readonly Type? TypeConverter;

        public PropertyAttribute(
            string name,
            PropertyMatchMode propertyMatchMode = PropertyMatchMode.Exact,
            Type? converter = null
        ) : base(name, propertyMatchMode)
        {
            TypeConverter = converter;
        }
    }
}