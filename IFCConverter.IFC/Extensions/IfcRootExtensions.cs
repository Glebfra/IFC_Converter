using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Geometry;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.IFC.Extensions
{
    public static class IfcRootExtensions
    {
        [Pure]
        public static Vector<double> ToVector(this IIfcCartesianPoint cartesianPoint)
        {
            return cartesianPoint.Coordinates.Cast<IIfcValue>().ToVector();
        }

        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IIfcCartesianPoint cartesianPoint)
            where TDimension : struct, IDimension
        {
            return cartesianPoint.Coordinates.Cast<IIfcValue>().ToFixedVector<TDimension>();
        }

        [Pure]
        public static Vector<double> ToVector(this IIfcDirection direction)
        {
            return new DenseVector(new[]
            {
                direction.X, direction.Y, direction.Z
            });
        }

        [Pure]
        public static FixedVector<Dim3> ToFixedVector(this IIfcDirection direction)
        {
            return FixedVector<Dim3>.Builder.Dense(new double[]
            {
                direction.X, direction.Y, direction.Z
            });
        }

        [Pure]
        public static Vector<double> ToVector(this IEnumerable<IfcParameterValue> values)
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return new DenseVector(doubles);
        }

        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IEnumerable<IfcParameterValue> values)
            where TDimension : struct, IDimension
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return FixedVector<TDimension>.Builder.Dense(doubles);
        }

        [Pure]
        public static Vector<double> ToVector(this IEnumerable<IIfcValue> values)
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return new DenseVector(doubles);
        }

        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IEnumerable<IIfcValue> values)
            where TDimension : struct, IDimension
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return FixedVector<TDimension>.Builder.Dense(doubles);
        }

        [Pure]
        public static Matrix<double> ToMatrix(this IIfcAxis2Placement axis2Placement)
        {
            switch (axis2Placement)
            {
                case IIfcAxis2Placement2D axis2Placement2D:
                    return axis2Placement2D.ToMatrix();
                case IIfcAxis2Placement3D axis2Placement3D:
                    return axis2Placement3D.ToMatrix();
                default:
                    throw new InvalidCastException($"Cannot cast {axis2Placement.GetType().Name} to " +
                                                   $"{nameof(IIfcAxis2Placement2D)} or {nameof(IIfcAxis2Placement3D)}.");
            }
        }

        [Pure]
        public static FixedMatrix<Dim4> ToFixedMatrix(this IIfcAxis2Placement axis2Placement)
        {
            switch (axis2Placement)
            {
                case IIfcAxis2Placement2D axis2Placement2D:
                    return axis2Placement2D.ToFixedMatrix();
                case IIfcAxis2Placement3D axis2Placement3D:
                    return axis2Placement3D.ToFixedMatrix();
                default:
                    throw new InvalidCastException($"Cannot cast {axis2Placement.GetType().Name} to " +
                                                   $"{nameof(IIfcAxis2Placement2D)} or {nameof(IIfcAxis2Placement3D)}.");
            }
        }

        [Pure]
        public static Matrix<double> ToMatrix(this IIfcAxis2Placement2D axis2Placement2D)
        {
            Vector<double> position = axis2Placement2D.Location.ToVector();
            Vector<double> refDirection = axis2Placement2D.RefDirection.ToVector().Normalize(2);
            Vector<double> upDirection = refDirection.CreateNormalVector().Normalize(2);
            Vector<double> axis = refDirection.CrossProduct(upDirection).Normalize(2);

            return MatrixExtensions.CreateTransition(position, refDirection, upDirection, axis);
        }

        [Pure]
        public static FixedMatrix<Dim4> ToFixedMatrix(this IIfcAxis2Placement2D axis2Placement2D)
        {
            FixedVector<Dim3> position = axis2Placement2D.Location.ToFixedVector<Dim3>();
            FixedVector<Dim3> refDirection = axis2Placement2D.RefDirection.ToFixedVector();
            FixedVector<Dim3> upDirection = refDirection.CreateNormalVector().Normalize();
            FixedVector<Dim3> axis = refDirection.CrossProduct(upDirection).Normalize();

            return FixedMatrix<Dim4>.Builder.CreateTransition(position, refDirection, upDirection, axis);
        }

        [Pure]
        public static Matrix<double> ToMatrix(this IIfcAxis2Placement3D axis2Placement3D)
        {
            Vector<double> axis = axis2Placement3D.Axis.ToVector();
            Vector<double> refDirection = axis2Placement3D.RefDirection.ToVector();
            Vector<double> upDirection = axis.CrossProduct(refDirection);
            Vector<double> position = axis2Placement3D.Location.ToVector();

            return MatrixExtensions.CreateTransition(position, refDirection, upDirection, axis);
        }

        [Pure]
        public static FixedMatrix<Dim4> ToFixedMatrix(this IIfcAxis2Placement3D axis2Placement3D)
        {
            FixedVector<Dim3> axis = axis2Placement3D.Axis.ToFixedVector();
            FixedVector<Dim3> refDirection = axis2Placement3D.RefDirection.ToFixedVector();
            FixedVector<Dim3> upDirection = axis.CrossProduct(refDirection);
            FixedVector<Dim3> position = axis2Placement3D.Location.ToFixedVector<Dim3>();

            return FixedMatrix<Dim4>.Builder.CreateTransition(position, refDirection, upDirection, axis);
        }

        [Pure]
        public static Dictionary<string, object> ToDictionary(this IIfcPropertySet ifcPropertySet)
        {
            Dictionary<string, object> properties = new Dictionary<string, object>();
            foreach (IIfcProperty hasProperty in ifcPropertySet.HasProperties)
            {
                if (hasProperty is IIfcPropertySingleValue singleValue)
                    properties[singleValue.Name] = singleValue.NominalValue;
                if (hasProperty is IIfcPropertyListValue listValue)
                    properties[listValue.Name] = listValue.ListValues;
            }

            return properties;
        }
        
        [Pure]
        public static IEnumerable<Vector<double>> GetCoordinates(this IIfcCartesianPointList3D pointList)
        {
            List<Vector<double>> result = new List<Vector<double>>();

            foreach (IItemSet<IfcLengthMeasure> ifcLengthMeasures in pointList.CoordList)
                result.Add(new DenseVector(new double[]
                {
                    ifcLengthMeasures[0], ifcLengthMeasures[1], ifcLengthMeasures[2]
                }));

            return result;
        }

        [Pure]
        public static IEnumerable<FixedVector<Dim3>> GetFixedCoordinates(this IIfcCartesianPointList3D pointList)
        {
            List<FixedVector<Dim3>> result = new List<FixedVector<Dim3>>();
            foreach (IItemSet<IfcLengthMeasure> ifcLengthMeasures in pointList.CoordList)
            {
                result.Add(FixedVector<Dim3>.Builder.Dense(new double[]
                {
                    ifcLengthMeasures[0], ifcLengthMeasures[1], ifcLengthMeasures[2]
                }));
            }

            return result;
        }
        
        [Pure]
        public static IEnumerable<IIfcRepresentationItem> GetRepresentationItems(this IIfcProduct source)
        {
            IIfcProductRepresentation representation = source.Representation;
            IEnumerable<IIfcRepresentation> representations = representation.Representations;
            return representations.SelectMany(ifcRepresentation => ifcRepresentation.Items);
        }

        [Pure]
        public static double GetLengthPower(this IModel model)
        {
            IfcSIUnit siUnit = model.Instances
                .OfType<IfcSIUnit>()
                .FirstOrDefault(unit => unit.UnitType == IfcUnitEnum.LENGTHUNIT);

            return siUnit?.Power ?? 1.0;
        }

        [Pure]
        public static Mesh GetMesh(this IIfcTriangulatedFaceSet triangulatedFaceSet)
        {
            Vector<double>[] vertices = triangulatedFaceSet.Coordinates.GetCoordinates().ToArray();
            Vector<double>[] normals = triangulatedFaceSet.Normals.Select(normal => normal.ToVector()).ToArray();
            int[][] triangles = triangulatedFaceSet.CoordIndex.Select(indices => indices.Select(index => (int)index - 1).ToArray()).ToArray();
            return new Mesh(vertices, triangles, normals);
        }
    }
}