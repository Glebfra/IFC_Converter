using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcUniversalExpansionJointEntity : IfcAbstractUniversalExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double Radius { get; protected set; }
    
        public IfcUniversalExpansionJointEntity(StartUniversalExpansionJointEntity universalExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(universalExpansion, nodeEntity, segmentEntities)
        {
            Length = universalExpansion.Length.SIProperty;
            Radius = segmentEntities[0].Diameter / 2;
        }
    }

    #endif
}