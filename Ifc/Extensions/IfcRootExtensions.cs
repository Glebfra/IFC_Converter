using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using MatrixExtensions = Utils.MatrixExtensions;

namespace Ifc.Extensions
{
    public static class IfcRootExtensions
    {
        [Pure]
        public static Vector<double> ToVector(this IIfcCartesianPoint cartesianPoint)
        {
            return ToVector(cartesianPoint.Coordinates.Cast<IIfcValue>());
        }

        [Pure]
        public static Vector<double> ToVector(this IIfcDirection direction)
        {
            return new DenseVector(new[]
            {
                direction.X,
                direction.Y,
                direction.Z
            });
        }

        [Pure]
        public static Vector<double> ToVector(this IEnumerable<IIfcValue> values)
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return new DenseVector(doubles);
        }

        [Pure]
        public static Matrix<double> ToMatrix(this IIfcAxis2Placement3D axis2Placement3D)
        {
            Vector<double> axis = ToVector(axis2Placement3D.Axis);
            Vector<double> refDirection = ToVector(axis2Placement3D.RefDirection);
            Vector<double> upDirection = axis.CrossProduct(refDirection);
            Vector<double> position = ToVector(axis2Placement3D.Location);

            return MatrixExtensions.CreateTransition(position, refDirection, upDirection, axis);
        }

        [Pure]
        public static Dictionary<string, object> ToDictionary(this IIfcPropertySet ifcPropertySet)
        {
            Dictionary<string, object> properties = new();
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
            List<Vector<double>> result = new();

            foreach (IItemSet<IfcLengthMeasure> ifcLengthMeasures in pointList.CoordList)
                result.Add(new DenseVector(new double[]
                {
                    ifcLengthMeasures[0],
                    ifcLengthMeasures[1],
                    ifcLengthMeasures[2]
                }));

            return result;
        }
    }
}