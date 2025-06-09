using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Extensions
{
    public static class IfcCartesianPointListExtensions
    {
        public static void AddCoords(this IfcCartesianPointList3D pointList3D, IEnumerable<XbimVector3D> vectors, ref int index)
        {
            IEnumerable<IEnumerable<double>> doubleVectors = vectors.Select(vector => vector.ToDoubleArray());
            AddCoords(pointList3D, doubleVectors, ref index);
        }

        public static void AddCoords(this IfcCartesianPointList3D pointList3D, XbimVector3D vector, ref int index)
        {
            IEnumerable<double> doubleVector = vector.ToDoubleArray();
            AddCoords(pointList3D, new[] { doubleVector }, ref index);
        }

        public static void AddCoords(this IfcCartesianPointList3D pointList3D, IEnumerable<double> vector, ref int index)
        {
            AddCoords(pointList3D, new[] { vector }, ref index);
        }

        public static void AddCoords(this IfcCartesianPointList3D pointList3D, IEnumerable<IEnumerable<double>> vectors, ref int index)
        {
            foreach (IEnumerable<double> vector in vectors)
            {
                IItemSet<IfcLengthMeasure> coordList = pointList3D.CoordList.GetAt(index++);
                coordList.AddRange(vector.Select(val => new IfcLengthMeasure(val)));
            }
        }
    }
}