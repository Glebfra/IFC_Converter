using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcGeometry
    {
        IEnumerable<IIfcBuilder> GeometryBuilders { get; }
        IIfcRepresentationContext RepresentationContext { get; }
        IColor Color { get; }

        IIfcShapeRepresentation CreateShapeRepresentation(IModel model);

        IIfcProductDefinitionShape CreateProductDefinitionShape(IModel model,
            IIfcShapeRepresentation shapeRepresentation);

        void AssignColor(IColor color);
    }
}