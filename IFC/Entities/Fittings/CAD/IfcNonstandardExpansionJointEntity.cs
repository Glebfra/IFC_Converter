using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    
    
    #else
    
    public sealed class IfcNonstandardExpansionJointEntity : IfcAbstractNonStandardExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double Radius { get; protected set; }
        
        public IfcNonstandardExpansionJointEntity(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonstandardExpansion, ifcNodeEntity, segmentEntities)
        {
            Length = nonstandardExpansion.Length.SIProperty;
            Radius = segmentEntities[0].Diameter / 2;
        }
    }

    #endif
}