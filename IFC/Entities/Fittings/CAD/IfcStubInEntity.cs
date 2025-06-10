using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using IFC.Tools.Geometry;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.CAD
{
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
}