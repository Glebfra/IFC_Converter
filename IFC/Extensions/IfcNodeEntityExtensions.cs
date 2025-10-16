using IFC.Entities;
using Xbim.Common.Geometry;

namespace IFC.Extensions
{
    public static class IfcNodeEntityExtensions
    {
        public static double GetDistanceToPoint(this IfcNodeEntity obj, XbimVector3D point)
        {
            return GetDisplacementToPoint(obj, point).Length;
        }
        
        public static XbimVector3D GetDisplacementToPoint(this IfcNodeEntity obj, XbimVector3D point)
        {
            return point - obj.ObjectMatrix3D.Translation;
        }

        public static double GetDistanceToNode(this IfcNodeEntity obj, IfcNodeEntity other)
        {
            return GetDisplacementToNode(obj, other).Length;
        }

        public static XbimVector3D GetDisplacementToNode(this IfcNodeEntity obj, IfcNodeEntity other)
        {
            return obj.ObjectMatrix3D.Translation - other.ObjectMatrix3D.Translation;
        }
    }
}