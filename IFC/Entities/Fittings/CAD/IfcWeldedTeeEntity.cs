using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcWeldedTeeEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }
        public override double Angle { get; protected set; }
        
        public IfcWeldedTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(teeEntity, nodeEntity, segmentEntities)
        {
            XbimVector3D right = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, ObjectMatrix3D.Translation).Normalized();
            Angle = ObjectMatrix3D.Forward.Angle(right);
            Length = teeEntity.HeaderLength.SIProperty;
            if (Length == 0) 
                Length = _HeadPipe.Diameter;
            Height = teeEntity.CrotchHeight.SIProperty + _BranchPipes[0].Diameter / 2;
        }
    }

    #endif
}