using System.Collections.Generic;
using System.Linq;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;

namespace IFC.Extensions
{
    public static class IfcCartesianPointList3DExtensions
    {
        public static IEnumerable<XbimVector3D> GetCoordinates(this IfcCartesianPointList3D pointList3D)
        {
            return pointList3D.CoordList
                .Select(coords => new XbimVector3D(coords[0], coords[1], coords[2]));
        }
    }
}