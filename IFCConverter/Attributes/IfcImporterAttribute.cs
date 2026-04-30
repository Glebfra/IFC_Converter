using System;

namespace IFCConverter.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    internal class IfcImporterAttribute : Attribute
    {
        public readonly Type Filter;
        
        public IfcImporterAttribute(Type filter)
        {
            Filter = filter;
        }
    }
}