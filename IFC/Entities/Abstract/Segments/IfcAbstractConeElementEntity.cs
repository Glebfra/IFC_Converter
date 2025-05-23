using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Segments
{
    public abstract class IfcAbstractConeElementEntity : IfcAbstractSegmentEntity
    {
        public abstract double SecondDiameter { get; set; }
        
        protected abstract int _NumSegments { get; set; }

        private StartConeElementEntity _coneElement;
        private IfcPipeSegment? _pipeSegment;
        
        protected IfcAbstractConeElementEntity(StartConeElementEntity coneElement, IfcNodeEntity[] nodeEntities) 
            : base(coneElement, nodeEntities)
        {
            _coneElement = coneElement;
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(
                model, Diameter / 2, SecondDiameter / 2, Length.Value, 
                XbimVector3D.Zero, _NumSegments, VectorExtensions.Right, VectorExtensions.Up
            );
            IfcShapeRepresentation shapeRepresentation = IfcVertexGeometry.CreateShapeRepresentation(model, facetedBrep);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            
            IfcColours.StyleItems(model, Colour, facetedBrep);

            _pipeSegment = model.Instances.New<IfcPipeSegment>(segment =>
            {
                segment.Tag = Tag;
                segment.Name = _coneElement.Name;
                segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
                segment.ObjectPlacement = objectPlacement.LocalPlacement;
                segment.Representation = shape;
            });
            AddProperties(model, _pipeSegment);

            return _pipeSegment;
        }
    }
}