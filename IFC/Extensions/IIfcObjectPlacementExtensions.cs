using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;

namespace IFC.Extensions
{
    public static class IIfcObjectPlacementExtensions
    {
        public static XbimMatrix3D ToObjectMatrix3D(this IIfcObjectPlacement objectPlacement)
        {
            XbimMatrix3D objectMatrix3D = objectPlacement.ToMatrix3D();
            objectMatrix3D = XbimMatrix3D.CreateWorld(objectMatrix3D.Translation, objectMatrix3D.Backward, objectMatrix3D.Up);
            return objectMatrix3D;
        }
    }
}