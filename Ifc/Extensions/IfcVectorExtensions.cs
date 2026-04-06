using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.TopologyResource;

namespace Ifc.Extensions
{
    internal static class IfcVectorExtensions
    {
        [Pure]
        public static IfcPolyLoop ToPolyLoop(this IEnumerable<Vector<double>> vectors, IModel model)
        {
            IEnumerable<IfcCartesianPoint> points = vectors.Select(vector => vector.ToCartesianPoint(model));
            return model.Instances.New<IfcPolyLoop>(loop => loop.Polygon.AddRange(points));
        }

        [Pure]
        public static IfcCartesianPointList3D ToCartesianPointList3D(this IEnumerable<Vector<double>> vectors,
            IModel model)
        {
            return model.Instances.New<IfcCartesianPointList3D>(list3D =>
            {
                int index = 0;
                foreach (Vector<double> vector in vectors)
                {
                    IItemSet<IfcLengthMeasure> itemSet = list3D.CoordList.GetAt(index++);
                    foreach (double coord in vector)
                        itemSet.Add(coord);
                }
            });
        }

        [Pure]
        public static IfcCartesianPoint ToCartesianPoint(this Vector<double> vector, IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(
                vector.GetX(),
                vector.GetY(),
                vector.GetZ()
            ));
        }

        [Pure]
        public static IfcDirection ToIfcDirection(this Vector<double> vector, IModel model)
        {
            return model.Instances.New<IfcDirection>(direction => direction.SetXYZ(
                vector.GetX(),
                vector.GetY(),
                vector.GetZ()
            ));
        }

        [Pure]
        public static IfcVector ToIfcVector(this Vector<double> vector, IModel model)
        {
            return model.Instances.New<IfcVector>(ifcVector =>
            {
                ifcVector.Orientation = vector.ToIfcDirection(model);
                ifcVector.Magnitude = vector.Norm(2);
            });
        }

        [Pure]
        public static IfcAxis1Placement CreateAxis1Placement(IModel model, Vector<double> position,
            Vector<double> direction)
        {
            return model.Instances.New<IfcAxis1Placement>(placement =>
            {
                placement.Axis = direction.ToIfcDirection(model);
                placement.Location = position.ToCartesianPoint(model);
            });
        }
    }
}