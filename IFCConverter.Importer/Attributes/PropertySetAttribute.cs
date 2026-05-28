using System;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
    internal class PropertySetAttribute : AbstractPropertyItemAttribute
    {
        public PropertySetAttribute(string name, PropertyMatchMode propertyMatchMode = PropertyMatchMode.Exact) 
            : base(name, propertyMatchMode)
        {
        }
    }
}