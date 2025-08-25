using System.Collections.Generic;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractCadBendEntity : IfcAbstractBendEntity
    {
        protected IfcAbstractCadBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }

        protected override IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            XbimVector3D displacement = CalculateDisplacement();
            
            IfcSweptDiskSolid pipeBend = IfcGeometry.CreateCircularBend(
                model, PipeRadius, BendRadius, Angle,
                displacement, VectorExtensions.Forward, VectorExtensions.Right
            );

            return new IfcRepresentationItem[] { pipeBend };
        }
    }
}