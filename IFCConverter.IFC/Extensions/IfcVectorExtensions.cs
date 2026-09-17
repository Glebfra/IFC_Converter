using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.TopologyResource;

namespace IFCConverter.IFC.Extensions
{
    internal static class IfcVectorExtensions
    {
        [Pure]
        public static IfcPolyLoop ToPolyLoop(this IEnumerable<FixedVector<Dim3>> vectors, IModel model)
        {
            IEnumerable<IfcCartesianPoint> points = vectors.Select(vector => vector.ToCartesianPoint(model));
            return model.Instances.New<IfcPolyLoop>(loop => loop.Polygon.AddRange(points));
        }

        [Pure]
        public static IfcCartesianPointList3D ToCartesianPointList3D(this IEnumerable<FixedVector<Dim3>> vectors,
            IModel model)
        {
            return model.Instances.New<IfcCartesianPointList3D>(list3D =>
            {
                int index = 0;
                foreach (FixedVector<Dim3> vector in vectors)
                {
                    IItemSet<IfcLengthMeasure> itemSet = list3D.CoordList.GetAt(index++);
                    foreach (double coord in vector)
                        itemSet.Add(coord);
                }
            });
        }

        [Pure]
        public static IfcCartesianPoint ToCartesianPoint(this FixedVector<Dim3> vector, IModel model)
        {
            return model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(
                vector.GetX(),
                vector.GetY(),
                vector.GetZ()
            ));
        }

        [Pure]
        public static IfcDirection ToIfcDirection(this FixedVector<Dim3> vector, IModel model)
        {
            return model.Instances.New<IfcDirection>(direction => direction.SetXYZ(
                vector.GetX(),
                vector.GetY(),
                vector.GetZ()
            ));
        }

        [Pure]
        public static IfcVector ToIfcVector(this FixedVector<Dim3> vector, IModel model)
        {
            return model.Instances.New<IfcVector>(ifcVector =>
            {
                ifcVector.Orientation = vector.ToIfcDirection(model);
                ifcVector.Magnitude = vector.L2Norm();
            });
        }

        [Pure]
        public static IfcAxis1Placement CreateAxis1Placement(IModel model, FixedVector<Dim3> position, FixedVector<Dim3> direction)
        {
            return model.Instances.New<IfcAxis1Placement>(placement =>
            {
                placement.Axis = direction.ToIfcDirection(model);
                placement.Location = position.ToCartesianPoint(model);
            });
        }
    }
}