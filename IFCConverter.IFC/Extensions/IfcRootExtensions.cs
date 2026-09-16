using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Geometry;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Extensions
{
    public static class IfcRootExtensions
    {
        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IIfcCartesianPoint cartesianPoint)
            where TDimension : struct, IDimension
        {
            return cartesianPoint.Coordinates.Cast<IIfcValue>().ToFixedVector<TDimension>();
        }

        [Pure]
        public static FixedVector<Dim3> ToFixedVector(this IIfcDirection direction)
        {
            return FixedVector<Dim3>.Builder.Dense(direction.X, direction.Y, direction.Z);
        }

        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IEnumerable<IfcParameterValue> values)
            where TDimension : struct, IDimension
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return FixedVector<TDimension>.Builder.Dense(doubles);
        }

        [Pure]
        public static FixedVector<TDimension> ToFixedVector<TDimension>(this IEnumerable<IIfcValue> values)
            where TDimension : struct, IDimension
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return FixedVector<TDimension>.Builder.Dense(doubles);
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
        public static FixedMatrix<Dim4> ToFixedMatrix(this IIfcAxis2Placement2D axis2Placement2D)
        {
            FixedVector<Dim3> position = axis2Placement2D.Location.ToFixedVector<Dim3>();
            FixedVector<Dim3> refDirection = axis2Placement2D.RefDirection.ToFixedVector();
            FixedVector<Dim3> upDirection = refDirection.CreateNormalVector().Normalize();
            FixedVector<Dim3> axis = refDirection.CrossProduct(upDirection).Normalize();

            return FixedMatrix<Dim4>.Builder.CreateTransition(position, refDirection, upDirection, axis);
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
        public static IEnumerable<FixedVector<Dim3>> GetFixedCoordinates(this IIfcCartesianPointList3D pointList)
        {
            List<FixedVector<Dim3>> result = new List<FixedVector<Dim3>>();
            foreach (IItemSet<IfcLengthMeasure> ifcLengthMeasures in pointList.CoordList)
            {
                result.Add(FixedVector<Dim3>.Builder.Dense(ifcLengthMeasures[0], ifcLengthMeasures[1], ifcLengthMeasures[2]));
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
            FixedVector<Dim3>[] vertices = triangulatedFaceSet.Coordinates.GetFixedCoordinates().ToArray();
            FixedVector<Dim3>[] normals = triangulatedFaceSet.Normals.Select(normal => normal.ToFixedVector<Dim3>()).ToArray();
            int[][] triangles = triangulatedFaceSet.CoordIndex.Select(indices => indices.Select(index => (int)index - 1).ToArray()).ToArray();
            return new Mesh(vertices, triangles, normals);
        }
    }
}