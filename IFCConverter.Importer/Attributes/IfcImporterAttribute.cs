using System;

namespace IFCConverter.Importer.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    internal class IfcImporterAttribute : Attribute
    {
        public readonly Type Filter;
        public readonly int Priority;

        public IfcImporterAttribute(Type filter, int priority = 0)
        {
            Filter = filter;
            Priority = priority;
        }
    }
}