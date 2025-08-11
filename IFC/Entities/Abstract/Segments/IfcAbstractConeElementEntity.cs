using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Segments
{
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
}