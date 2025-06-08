using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcCapEntity : IfcAbstractCapEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        
        public IfcCapEntity(StartCapEntity capEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(capEntity, nodeEntity, segmentEntities)
        {
            Diameter = segmentEntities[0].Diameter;
            Length = Diameter / 2;
        }
    }
}