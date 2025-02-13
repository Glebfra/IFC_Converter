using Xbim.Common.Geometry;

namespace IFC.Extensions;

public static class VectorExtensions
{
    public static double SignedAngle(this XbimVector3D first, XbimVector3D second)
    {
         return Math.Acos(XbimVector3D.DotProduct(first, second) / (first.Length * second.Length));
    }
}