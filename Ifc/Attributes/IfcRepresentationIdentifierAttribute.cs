using System;
using Ifc.API;

namespace Ifc.Attributes
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