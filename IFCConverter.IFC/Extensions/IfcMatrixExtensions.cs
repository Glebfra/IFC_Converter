using System.Diagnostics.Contracts;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace IFCConverter.IFC.Extensions
{
    internal static class IfcMatrixExtensions
    {
        [Pure]
        public static IfcAxis2Placement3D ToAxis2Placement3D(this FixedMatrix<Dim4> matrix, IModel model)
        {
            IfcCartesianPoint location = matrix.GetTranslation().ToCartesianPoint(model);
            IfcDirection axis = matrix.GetZ().ToCartesian().ToIfcDirection(model);
            IfcDirection refDirection = matrix.GetX().ToCartesian().ToIfcDirection(model);

            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = location;
                placement3D.Axis = axis;
                placement3D.RefDirection = refDirection;
            });
        }

        [Pure]
        public static IfcAxis2Placement2D ToAxis2Placement2D(this FixedMatrix<Dim4> matrix, IModel model)
        {
            IfcCartesianPoint location = matrix.GetTranslation().ToCartesianPoint(model);
            IfcDirection refDirection = matrix.GetX().ToCartesian().ToIfcDirection(model);

            return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = location;
                placement2D.RefDirection = refDirection;
            });
        }

        [Pure]
        public static IfcObjectPlacement ToIfcObjectPlacement(this FixedMatrix<Dim4> matrix, IModel model)
        {
            IfcAxis2Placement3D axis2Placement3D = matrix.ToAxis2Placement3D(model);
            return model.Instances.New<IfcLocalPlacement>(placement =>
            {
                placement.RelativePlacement = axis2Placement3D;
            });
        }
    }
}