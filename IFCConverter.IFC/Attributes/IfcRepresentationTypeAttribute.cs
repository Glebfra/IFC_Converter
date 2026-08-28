using System;
using IFCConverter.IFC.API;

namespace IFCConverter.IFC.Attributes
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