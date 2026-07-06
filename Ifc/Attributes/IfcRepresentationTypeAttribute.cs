using System;
using Ifc.API;

namespace Ifc.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class IfcRepresentationTypeAttribute : Attribute
    {
        public readonly IfcRepresentationType IfcRepresentationType;

        public IfcRepresentationTypeAttribute(IfcRepresentationType ifcRepresentationType)
        {
            IfcRepresentationType = ifcRepresentationType;
        }
    }
}