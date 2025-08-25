using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Extensions
{
    public static class IfcDirectionExtensions
    {
        public static void SetVector(this IfcDirection direction, XbimVector3D vector3D)
        {
            direction.SetXYZ(vector3D.X, vector3D.Y, vector3D.Z);
        }
    }
}