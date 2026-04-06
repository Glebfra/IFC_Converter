using System.Diagnostics.Contracts;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace Ifc.Extensions
{
    internal static class IfcMatrixExtensions
    {
        [Pure]
        public static IfcAxis2Placement3D ToAxis2Placement3D(this Matrix<double> matrix, IModel model)
        {
            IfcCartesianPoint location = matrix.GetOffset().ToCartesianPoint(model);
            IfcDirection axis = matrix.GetZ().ToIfcDirection(model);
            IfcDirection refDirection = matrix.GetX().ToIfcDirection(model);

            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = location;
                placement3D.Axis = axis;
                placement3D.RefDirection = refDirection;
            });
        }

        [Pure]
        public static IfcAxis2Placement2D ToAxis2Placement2D(this Matrix<double> matrix, IModel model)
        {
            IfcCartesianPoint location = matrix.GetOffset().ToCartesianPoint(model);
            IfcDirection refDirection = matrix.GetX().ToIfcDirection(model);

            return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = location;
                placement2D.RefDirection = refDirection;
            });
        }

        [Pure]
        public static IfcObjectPlacement ToIfcObjectPlacement(this Matrix<double> matrix, IModel model)
        {
            IfcAxis2Placement3D axis2Placement3D = matrix.ToAxis2Placement3D(model);
            return model.Instances.New<IfcLocalPlacement>(placement =>
            {
                placement.RelativePlacement = axis2Placement3D;
            });
        }
    }
}