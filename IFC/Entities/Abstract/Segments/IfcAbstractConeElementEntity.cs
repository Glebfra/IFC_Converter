using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Segments;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Segments
{
    #if NEW
    
    public abstract class IfcAbstractConeElementEntity : IfcAbstractSegmentEntity
    {
        public abstract int NumSegments { get; }
        public abstract ActionProperty<double> SecondDiameter { get; }
        
        protected IfcAbstractConeElementEntity(XbimMatrix3D matrix3D, double length) : base(matrix3D, length) { }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeSegment pipeSegment = CreateIfcEntity<IfcPipeSegment>(model, IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT);
            return pipeSegment;
        }

        private new T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model, pipeSegmentType);
            
            IfcRepresentationItem representationItem = CreatePipeShape(model);
            AddShapeRepresentation(model, pipeSegment, representationItem);
            return pipeSegment;
        }
        
        private IfcRepresentationItem CreatePipeShape(IModel model)
        {
            IfcFacetedBrep facetedBrep = IfcVertexGeometry.CreateClippedCone(
                model, Diameter / 2, SecondDiameter / 2, Length, 
                XbimVector3D.Zero, NumSegments, VectorExtensions.Right, VectorExtensions.Up
            );

            return facetedBrep;
        }
    }
    
    #else
    
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
                model, Diameter / 2, SecondDiameter / 2, RealLength.Value, 
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

    #endif
}