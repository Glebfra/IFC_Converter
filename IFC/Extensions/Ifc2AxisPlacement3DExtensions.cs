using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;

namespace IFC.Extensions
{
    public static class Ifc2AxisPlacement3DExtensions
    {
        public static XbimMatrix3D ToObjectMatrix3D(this IIfcAxis2Placement3D axis2Placement3D)
        {
            XbimMatrix3D objectMatrix3D = axis2Placement3D.ToMatrix3D();
            objectMatrix3D = XbimMatrix3D.CreateWorld(objectMatrix3D.Translation, objectMatrix3D.Backward, objectMatrix3D.Up);
            return objectMatrix3D;
        }
    }
}