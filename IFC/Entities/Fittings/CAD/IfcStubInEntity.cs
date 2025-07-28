using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW

    public sealed class IfcStubInEntity : IfcAbstractTeeEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> BranchDiameter { get; }
        public override ActionProperty<double> HeadDiameter { get; }
        public override ActionProperty<double> Height { get; }
        public override ActionProperty<double> Angle { get; }
        
        public IfcStubInEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double branchDiameter, double headDiameter, double height, double angle) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            BranchDiameter = branchDiameter;
            HeadDiameter = headDiameter;
            Height = height;
            Angle = angle;
        }
    }
    
    #else
    
    public sealed class IfcStubInEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }
        public override double Angle { get; protected set; }
        
        public IfcStubInEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(teeEntity, nodeEntity, segmentEntities)
        {
            XbimVector3D right = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, ObjectMatrix3D.Translation).Normalized();
            Angle = ObjectMatrix3D.Forward.Angle(right);
            Length = _HeadPipe.Diameter;
            Height = _BranchPipes[0].Diameter / 2;
        }
    }

    #endif
}