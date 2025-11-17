using Xbim.Common.Geometry;

namespace IFC.Entities
{
    public class IfcNodeEntity
    {
        public int ID { get; }
        public XbimMatrix3D ObjectMatrix3D { get; }

        public IfcNodeEntity(XbimMatrix3D objectMatrix3D)
        {
            ObjectMatrix3D = objectMatrix3D;
            ID = 0;
        }

        public IfcNodeEntity(XbimMatrix3D objectMatrix3D, int id)
        {
            ObjectMatrix3D = objectMatrix3D;
            ID = id;
        }

        public bool Equals(IfcNodeEntity other)
        {
            return ObjectMatrix3D.Translation == other.ObjectMatrix3D.Translation;
        }
        
        public static XbimVector3D GetDisplacementToPoint(IfcNodeEntity obj, XbimVector3D point) => point - obj.ObjectMatrix3D.Translation;
        public static double GetDistanceToPoint(IfcNodeEntity obj, XbimVector3D point) => GetDisplacementToPoint(obj, point).Length;
        public XbimVector3D GetDisplacementToPoint(XbimVector3D point) => GetDisplacementToPoint(this, point);
        public double GetDistanceToPoint(XbimVector3D point) => GetDistanceToPoint(this, point);
        
        public static XbimVector3D GetDisplacementToNode(IfcNodeEntity obj, IfcNodeEntity other) => obj.ObjectMatrix3D.Translation - other.ObjectMatrix3D.Translation;
        public static double GetDistanceToNode(IfcNodeEntity obj, IfcNodeEntity other) => GetDisplacementToNode(obj, other).Length;
        public XbimVector3D GetDisplacementToNode(IfcNodeEntity other) => GetDisplacementToNode(this, other);
        public double GetDistanceToNode(IfcNodeEntity other) => GetDistanceToNode(this, other);
    }
}