using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcGeometry
    {
        public IEnumerable<IIfcBuilder> GeometryBuilders { get; }
        public IIfcRepresentationContext? RepresentationContext { get; }
        public IColor? Color { get; }

        public IIfcShapeRepresentation CreateShapeRepresentation(IModel model);

        public IIfcProductDefinitionShape CreateProductDefinitionShape(IModel model,
            IIfcShapeRepresentation shapeRepresentation);

        public void AssignColor(IColor color);
    }
}