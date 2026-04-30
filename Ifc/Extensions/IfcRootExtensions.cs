using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Utils;
using Xbim.Ifc4.Interfaces;
using MatrixExtensions = Utils.MatrixExtensions;

namespace Ifc.Extensions
{
    public static class IfcRootExtensions
    {
        public static Vector<double> ToVector(this IIfcCartesianPoint cartesianPoint)
        {
            return ToVector(cartesianPoint.Coordinates.Cast<IIfcValue>());
        }

        public static Vector<double> ToVector(this IIfcDirection direction)
        {
            return new DenseVector(new double[]
            {
                direction.X,
                direction.Y,
                direction.Z
            });
        }

        public static Vector<double> ToVector(this IEnumerable<IIfcValue> values)
        {
            double[] doubles = values.Select(value => Convert.ToDouble(value.Value)).ToArray();
            return new DenseVector(doubles);
        }

        public static Matrix<double> ToMatrix(this IIfcAxis2Placement3D axis2Placement3D)
        {
            Vector<double> axis = ToVector(axis2Placement3D.Axis);
            Vector<double> refDirection = ToVector(axis2Placement3D.RefDirection);
            Vector<double> upDirection = axis.CrossProduct(refDirection);
            Vector<double> position = ToVector(axis2Placement3D.Location);

            return MatrixExtensions.CreateTransition(position, refDirection, upDirection, axis);
        }

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
    }
}