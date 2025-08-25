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
    }
}