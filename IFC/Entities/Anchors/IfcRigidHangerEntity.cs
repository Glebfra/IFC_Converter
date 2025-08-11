using IFC.Entities.Abstract.Anchors;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors
{
    public class IfcRigidHangerEntity : IfcAbstractRigidHangerEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }
        public override ActionProperty<double> Height { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcRigidHangerEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double diameter, double height, int numSegments) 
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            Diameter = diameter;
            Height = height;
            NumSegments = numSegments;
        }
    }
}