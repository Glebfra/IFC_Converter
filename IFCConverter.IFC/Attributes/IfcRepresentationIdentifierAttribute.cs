using System;
using IFCConverter.IFC.API;

namespace IFCConverter.IFC.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class IfcRepresentationIdentifierAttribute : Attribute
    {
        public readonly IfcRepresentationIdentifier RepresentationIdentifier;

        public IfcRepresentationIdentifierAttribute(IfcRepresentationIdentifier representationIdentifier)
        {
            RepresentationIdentifier = representationIdentifier;
        }
    }
}