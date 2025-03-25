using System;
using Start.API;

namespace IFC
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IfcEntityTypeAttribute : Attribute
    {
        public StartElementType[] Types { get; }
        public bool IsVertex { get; }

        public IfcEntityTypeAttribute(bool isVertex, params StartElementType[] types)
        {
            IsVertex = isVertex;
            Types = types;
        }
    }
}