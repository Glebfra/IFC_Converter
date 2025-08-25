using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexBendEntity : IfcAbstractBendEntity
    {
        public abstract int NumSegments { get; }
        public abstract double AngleStep { get; }
        public abstract double BendAngleStep { get; }
        
        protected IfcAbstractVertexBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        protected override IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D displacement = CalculateDisplacement();
            IfcAxisSettings axisSettings = new IfcAxisSettings(displacement, VectorExtensions.X, VectorExtensions.Z);

            IfcFacetedBrep torus = IfcVertexGeometry.CreateTorus(
                model,
                BendRadius,
                PipeRadius,
                Angle,
                NumSegments,
                axisSettings
            );

            return new IfcRepresentationItem[] { torus };
        }
    }
}