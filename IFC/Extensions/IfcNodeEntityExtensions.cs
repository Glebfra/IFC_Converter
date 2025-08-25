using IFC.Entities;
using Xbim.Common.Geometry;

namespace IFC.Extensions
{
    public static class IfcNodeEntityExtensions
    {
        public static double GetDistanceToAnotherNode(this IfcNodeEntity obj, IfcNodeEntity other)
        {
            return GetDistanceBetweenTwoNodes(obj, other);
        }

        public static double GetDistanceBetweenTwoNodes(IfcNodeEntity first, IfcNodeEntity second)
        {
            return GetDisplacementBetweenTwoNodes(first, second).Modulus;
        }

        public static XbimVector3D GetDisplacementToAnotherNode(this IfcNodeEntity obj, IfcNodeEntity other)
        {
            return GetDisplacementBetweenTwoNodes(obj, other);
        }

        public static XbimVector3D GetDisplacementBetweenTwoNodes(IfcNodeEntity first, IfcNodeEntity second)
        {
            return second.ObjectMatrix3D.Translation - first.ObjectMatrix3D.Translation;
        }
    }
}