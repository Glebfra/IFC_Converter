using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools.Geometry
{
    public struct IfcObjectPlacement
    {
        public IfcCartesianPoint Point;
        public IfcDirection? Forward;
        public IfcDirection? Right;
        public IfcAxis2Placement3D Axis2Placement3D;
        public IfcLocalPlacement LocalPlacement;
    }
}