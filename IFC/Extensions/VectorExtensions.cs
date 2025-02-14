using Xbim.Common.Geometry;

namespace IFC.Extensions;

public static class VectorExtensions
{
    public static XbimVector3D Right => new XbimVector3D(1, 0, 0);
    public static XbimVector3D Up => new XbimVector3D(0, 1, 0);
    public static XbimVector3D Forward => new XbimVector3D(0, 0, 1);
    
    public static double SignedAngle(this XbimVector3D first, XbimVector3D second)
    {
         return Math.Acos(XbimVector3D.DotProduct(first, second) / (first.Length * second.Length));
    }
}