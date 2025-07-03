using System.Collections.Generic;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Extensions
{
    public static class IfcProductExtensions
    {
        public static IEnumerable<IfcRepresentationItem> GetRepresentationItems(this IfcProduct product)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            foreach (IfcRepresentation representation in product.Representation.Representations)
            {
                representationItems.AddRange(representation.Items);
            }

            return representationItems;
        }
    }
}